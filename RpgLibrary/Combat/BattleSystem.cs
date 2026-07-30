using System;
using System.Collections.Generic;
using RpgLibrary.Contracts;
using RpgLibrary.Exceptions;

namespace RpgLibrary.Combat
{
    // BattleSystem — turn-based combat orchestrator.
    //
    // Knows the RULES of turn-based combat:
    //   1. Every round, sort combatants by Speed (fastest first).
    //   2. Each living combatant acts once.
    //   3. Party actions come from a menu the UI presents.
    //   4. Enemy actions come from a pluggable AI strategy.
    //   5. Battle ends when one side has no living members,
    //      OR when MaxRounds is exceeded (Timeout).
    //
    // Knows NOTHING about:
    //   - How to draw the menu     (that's IBattleUI)
    //   - How enemies decide       (that's IEnemyStrategy)
    //   - What targets a skill wants (that's Skill.Target)
    //   - How much to charge each turn (that's BattleSettings)
    public class BattleSystem
    {
        private readonly List<PartyMember> _party;
        private readonly List<ICombatant> _enemies;
        private readonly IBattleUI _ui;
        private readonly IEnemyStrategy _enemyStrategy;
        private readonly BattleSettings _settings;

        public int Round { get; private set; }
        public BattleOutcome Outcome { get; private set; } = BattleOutcome.Undecided;
        public bool PartyWon => Outcome == BattleOutcome.Victory;
        public bool BattleOver => Outcome != BattleOutcome.Undecided;

        public event Action<BattleState>? RoundStarted;
        public event Action<ICombatant>? TurnStarted;
        public event Action<BattleOutcome, int>? BattleEnded;

        public BattleSystem(
            List<PartyMember> party,
            List<ICombatant> enemies,
            IBattleUI ui,
            IEnemyStrategy enemyStrategy,
            BattleSettings settings)
        {
            if (party == null) throw new ArgumentNullException(nameof(party));
            if (enemies == null) throw new ArgumentNullException(nameof(enemies));
            if (ui == null) throw new ArgumentNullException(nameof(ui),
                "BattleSystem no longer defaults to ConsoleBattleUI — pass one explicitly.");
            if (enemyStrategy == null) throw new ArgumentNullException(nameof(enemyStrategy));
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            if (party.Count == 0) throw new ArgumentException("Party cannot be empty.", nameof(party));
            if (enemies.Count == 0) throw new ArgumentException("Enemy list cannot be empty.", nameof(enemies));

            _party = party;
            _enemies = enemies;
            _ui = ui;
            _enemyStrategy = enemyStrategy;
            _settings = settings;
        }

        // ---------- MAIN LOOP ----------
        public void Run()
        {
            // Route every domain-class log message through the UI while
            // this battle is running. Restore the previous sink at the
            // end so we play nice with other consumers.
            Action<string> previousSink = CombatLog.Sink;
            CombatLog.Sink = _ui.ShowMessage;

            try
            {
                _ui.ShowIntro(BuildState());

                while (PartyAlive() && EnemiesAlive())
                {
                    // Check the round cap BEFORE incrementing so Round
                    // does not overshoot on a timeout.
                    if (_settings.MaxRounds > 0 && Round >= _settings.MaxRounds)
                    {
                        Outcome = BattleOutcome.Timeout;
                        break;
                    }

                    Round++;

                    BattleState state = BuildState();
                    _ui.ShowRoundStart(state);
                    RoundStarted?.Invoke(state);

                    foreach (ICombatant actor in state.TurnOrder)
                    {
                        if (!actor.IsAlive()) continue;
                        if (!PartyAlive() || !EnemiesAlive()) break;

                        PartyMember? member = FindPartyMember(actor);
                        bool isHero = member != null;

                        _ui.ShowActorTurn(actor, isHero);
                        TurnStarted?.Invoke(actor);

                        if (isHero)
                        {
                            PlayerTurn(member!);
                            if (member!.Ultimate != null)
                                member.Ultimate.Charge(_settings.UltimateChargePerTurn);
                        }
                        else
                        {
                            EnemyTurn(actor);
                        }
                    }
                }

                // If the loop ended without a timeout, decide the outcome
                // from who is still standing.
                if (Outcome == BattleOutcome.Undecided)
                {
                    if (!EnemiesAlive() && PartyAlive())      Outcome = BattleOutcome.Victory;
                    else if (!PartyAlive() && EnemiesAlive()) Outcome = BattleOutcome.Defeat;
                    else                                        Outcome = BattleOutcome.Timeout;
                }

                _ui.ShowBattleEnd(Outcome, Round);
                BattleEnded?.Invoke(Outcome, Round);
            }
            finally
            {
                CombatLog.Sink = previousSink;
            }
        }

        // ---------- STATE ----------
        private BattleState BuildState()
        {
            return new BattleState(Round, _party, _enemies, BuildTurnOrder());
        }

        private List<ICombatant> BuildTurnOrder()
        {
            List<ICombatant> all = new List<ICombatant>();

            foreach (PartyMember m in _party)
            {
                if (!_settings.RemoveKOFromTurnOrder || m.Combatant.IsAlive())
                    all.Add(m.Combatant);
            }
            foreach (ICombatant e in _enemies)
            {
                if (!_settings.RemoveKOFromTurnOrder || e.IsAlive())
                    all.Add(e);
            }

            all.Sort((a, b) => b.Speed - a.Speed);
            return all;
        }

        private PartyMember? FindPartyMember(ICombatant actor)
        {
            foreach (PartyMember m in _party)
            {
                if (ReferenceEquals(m.Combatant, actor)) return m;
            }
            return null;
        }

        private bool PartyAlive()
        {
            foreach (PartyMember m in _party)
                if (m.Combatant.IsAlive()) return true;
            return false;
        }

        private bool EnemiesAlive()
        {
            foreach (ICombatant e in _enemies)
                if (e.IsAlive()) return true;
            return false;
        }

        // ---------- PLAYER TURN ----------
        private void PlayerTurn(PartyMember member)
        {
            List<BattleActionOption> options = BuildActionMenu(member);
            BattleActionOption? chosen = _ui.ChooseAction(member, options);
            if (chosen == null) return;

            if (chosen.Kind == BattleActionKind.Attack)   DoAttack(member);
            else if (chosen.Kind == BattleActionKind.Skill)    DoSkill(member);
            else if (chosen.Kind == BattleActionKind.Ultimate) DoUltimate(member);
        }

        // Build the list of enabled actions for this hero.
        // Defend and Flee are not offered yet — implement them in
        // BuildActionMenu when the mechanics exist.
        private List<BattleActionOption> BuildActionMenu(PartyMember member)
        {
            List<BattleActionOption> list = new List<BattleActionOption>();

            list.Add(new BattleActionOption(BattleActionKind.Attack, "Attack"));

            if (member.Skills.Count > 0)
                list.Add(new BattleActionOption(BattleActionKind.Skill, "Skill"));

            if (member.Ultimate != null)
            {
                bool ready = member.Ultimate.IsCharged;
                string detail = ready
                    ? "READY"
                    : $"{member.Ultimate.CurrentCharge}/{member.Ultimate.MaxCharge}";

                list.Add(new BattleActionOption(
                    BattleActionKind.Ultimate,
                    $"Ultimate ({member.Ultimate.Name})",
                    enabled: ready,
                    detail: detail));
            }

            return list;
        }

        private void DoAttack(PartyMember member)
        {
            ICombatant? target = PickEnemyTarget();
            if (target != null) member.Combatant.BasicAttack(target);
        }

        private void DoSkill(PartyMember member)
        {
            // The UI's ChooseSkill takes a concrete List<Skill>, so
            // copy the read-only view into one before passing it in.
            List<Skill> skills = new List<Skill>(member.Skills);
            Skill? skill = _ui.ChooseSkill(skills);
            if (skill != null) ApplySkill(member, skill);
        }

        private void DoUltimate(PartyMember member)
        {
            if (member.Ultimate == null) return;

            try
            {
                ApplySkill(member, member.Ultimate);
            }
            catch (UltimateNotChargedException ex)
            {
                _ui.ShowMessage($"  [!] {ex.Message}");
            }
        }

        // Apply a skill by asking it who it targets.
        // No `is HealSkill` type checks anywhere.
        private void ApplySkill(PartyMember member, Skill skill)
        {
            ICombatant caster = member.Combatant;

            if (skill.Target == TargetType.Self)
            {
                skill.Use(caster, caster);
            }
            else if (skill.Target == TargetType.SingleEnemy)
            {
                ICombatant? t = PickEnemyTarget();
                if (t != null) skill.Use(caster, t);
            }
            else if (skill.Target == TargetType.SingleAlly)
            {
                ICombatant? t = PickAllyTarget(caster);
                if (t != null) skill.Use(caster, t);
            }
            else if (skill.Target == TargetType.AllEnemies)
            {
                foreach (ICombatant e in _enemies)
                    if (e.IsAlive()) skill.Use(caster, e);
            }
            else if (skill.Target == TargetType.AllAllies)
            {
                foreach (PartyMember m in _party)
                    if (m.Combatant.IsAlive()) skill.Use(caster, m.Combatant);
            }
        }

        private ICombatant? PickEnemyTarget()
        {
            List<ICombatant> alive = new List<ICombatant>();
            foreach (ICombatant e in _enemies)
                if (e.IsAlive()) alive.Add(e);
            return _ui.ChooseTarget(alive, "Choose enemy");
        }

        private ICombatant? PickAllyTarget(ICombatant self)
        {
            List<ICombatant> alive = new List<ICombatant>();
            foreach (PartyMember m in _party)
                if (m.Combatant.IsAlive() && !ReferenceEquals(m.Combatant, self))
                    alive.Add(m.Combatant);
            return _ui.ChooseTarget(alive, "Choose ally");
        }

        // ---------- ENEMY TURN ----------
        private void EnemyTurn(ICombatant actor)
        {
            List<ICombatant> aliveAllies = new List<ICombatant>();
            List<ICombatant> aliveHeroes = new List<ICombatant>();

            foreach (ICombatant e in _enemies)
                if (e.IsAlive()) aliveAllies.Add(e);
            foreach (PartyMember m in _party)
                if (m.Combatant.IsAlive()) aliveHeroes.Add(m.Combatant);

            _enemyStrategy.TakeTurn(actor, aliveAllies, aliveHeroes);
        }
    }
}
