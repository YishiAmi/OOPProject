using System;
using System.Collections.Generic;
using System.Linq;
using RpgLibrary.Contracts;
using RpgLibrary.Exceptions;

namespace RpgLibrary.Combat
{
    // PartyMember bundles a hero-side combatant with the skills the player
    // can pick during that hero's turn, plus an optional Ultimate.
    public class PartyMember
    {
        public ICombatant Combatant { get; }
        public List<Skill> Skills { get; }
        public UltimateSkill? Ultimate { get; }

        public PartyMember(ICombatant combatant,
                           IEnumerable<Skill>? skills = null,
                           UltimateSkill? ultimate = null)
        {
            Combatant = combatant;
            Skills = skills?.ToList() ?? new List<Skill>();
            Ultimate = ultimate;
        }
    }
   
    public class BattleSystem
    {
        private readonly List<PartyMember> _party;
        private readonly List<ICombatant> _enemies;
        private readonly IBattleUI _ui;
        private readonly IEnemyStrategy _enemyStrategy;
        private readonly BattleSettings _settings;

        public int Round { get; private set; }
        public bool PartyWon { get; private set; }
        public bool BattleOver { get; private set; }

        public event Action<BattleState>? RoundStarted;
        public event Action<ICombatant>? TurnStarted;
        public event Action<bool, int>? BattleEnded;

        public BattleSystem(
            IEnumerable<PartyMember> party,
            IEnumerable<ICombatant> enemies,
            IBattleUI? ui = null,
            IEnemyStrategy? enemyStrategy = null,
            BattleSettings? settings = null)
        {
            _party = party.ToList();
            _enemies = enemies.ToList();
            _ui = ui ?? new ConsoleBattleUI();
            _enemyStrategy = enemyStrategy ?? new RandomTargetStrategy();
            _settings = settings ?? new BattleSettings();
        }

        // ---------- MAIN LOOP ----------
        public void Run()
        {
            _ui.ShowIntro(BuildState());

            while (PartyAlive() && EnemiesAlive())
            {
                Round++;
                if (_settings.MaxRounds > 0 && Round > _settings.MaxRounds) break;

                var state = BuildState();
                _ui.ShowRoundStart(state);
                RoundStarted?.Invoke(state);

                foreach (var actor in state.TurnOrder)
                {
                    if (!actor.IsAlive()) continue;
                    if (!PartyAlive() || !EnemiesAlive()) break;

                    bool isPartyMember = _party.Any(p => ReferenceEquals(p.Combatant, actor));
                    _ui.ShowActorTurn(actor, isPartyMember);
                    TurnStarted?.Invoke(actor);

                    if (isPartyMember)
                    {
                        var member = _party.First(p => ReferenceEquals(p.Combatant, actor));
                        PlayerTurn(member);
                        member.Ultimate?.Charge(_settings.UltimateChargePerTurn);
                    }
                    else
                    {
                        _enemyStrategy.TakeTurn(
                            actor,
                            _enemies.Where(e => e.IsAlive()).ToList(),
                            _party.Select(p => p.Combatant).Where(c => c.IsAlive()).ToList());
                    }
                }
            }

            BattleOver = true;
            PartyWon = PartyAlive();
            _ui.ShowBattleEnd(PartyWon, Round);
            BattleEnded?.Invoke(PartyWon, Round);
        }

        // ---------- STATE ----------
        private BattleState BuildState()
        {
            return new BattleState(Round, _party, _enemies, BuildTurnOrder());
        }

        private List<ICombatant> BuildTurnOrder()
        {
            var all = _party.Select(p => p.Combatant).Concat(_enemies);
            if (_settings.RemoveKOFromTurnOrder)
                all = all.Where(c => c.IsAlive());
            return all.OrderByDescending(c => c.Speed).ToList();
        }

        private bool PartyAlive()   => _party.Any(p => p.Combatant.IsAlive());
        private bool EnemiesAlive() => _enemies.Any(e => e.IsAlive());

        // ---------- PLAYER TURN ----------
        private void PlayerTurn(PartyMember member)
        {
            var options = BuildActionMenu(member);
            var chosen = _ui.ChooseAction(member, options);
            if (chosen == null) return;

            switch (chosen.Kind)
            {
                case BattleActionKind.Attack:   DoAttack(member); break;
                case BattleActionKind.Skill:    DoSkill(member); break;
                case BattleActionKind.Ultimate: DoUltimate(member); break;
                case BattleActionKind.Defend:   _ui.ShowMessage($"  {member.Combatant.Name} braces for impact."); break;
                case BattleActionKind.Flee:     _ui.ShowMessage($"  {member.Combatant.Name} tries to flee!"); break;
            }
        }

        private IReadOnlyList<BattleActionOption> BuildActionMenu(PartyMember member)
        {
            var list = new List<BattleActionOption>();
            list.Add(new BattleActionOption(BattleActionKind.Attack, "Attack"));
            if (member.Skills.Count > 0)
                list.Add(new BattleActionOption(BattleActionKind.Skill, "Skill"));
            if (member.Ultimate != null)
            {
                string detail = member.Ultimate.IsCharged
                    ? "READY"
                    : $"{member.Ultimate.CurrentCharge}/{member.Ultimate.MaxCharge}";
                list.Add(new BattleActionOption(
                    BattleActionKind.Ultimate,
                    $"Ultimate ({member.Ultimate.Name})",
                    enabled: member.Ultimate.IsCharged,
                    detail: detail));
            }
            return list;
        }

        private void DoAttack(PartyMember member)
        {
            var target = PickByTargetType(member, TargetType.SingleEnemy);
            if (target == null) return;
            member.Combatant.BasicAttack(target);
        }

        private void DoSkill(PartyMember member)
        {
            var skill = _ui.ChooseSkill(member.Skills);
            if (skill == null) return;
            ApplySkill(member, skill);
        }

        private void DoUltimate(PartyMember member)
        {
            if (member.Ultimate == null) return;
            try { ApplySkill(member, member.Ultimate); }
            catch (UltimateNotChargedException ex) { _ui.ShowMessage($"  [!] {ex.Message}"); }
        }

        private void ApplySkill(PartyMember member, Skill skill)
        {
            var caster = member.Combatant;
            switch (skill.Target)
            {
                case TargetType.Self:
                    skill.Use(caster, caster);
                    break;

                case TargetType.SingleAlly:
                {
                    var t = PickByTargetType(member, TargetType.SingleAlly);
                    if (t != null) skill.Use(caster, t);
                    break;
                }

                case TargetType.SingleEnemy:
                {
                    var t = PickByTargetType(member, TargetType.SingleEnemy);
                    if (t != null) skill.Use(caster, t);
                    break;
                }

                case TargetType.AllAllies:
                    foreach (var ally in _party.Select(p => p.Combatant).Where(c => c.IsAlive()))
                        skill.Use(caster, ally);
                    break;

                case TargetType.AllEnemies:
                    foreach (var foe in _enemies.Where(e => e.IsAlive()))
                        skill.Use(caster, foe);
                    break;
            }
        }

        private ICombatant? PickByTargetType(PartyMember member, TargetType type)
        {
            switch (type)
            {
                case TargetType.SingleEnemy:
                    return _ui.ChooseTarget(_enemies.Where(e => e.IsAlive()).ToList(), "Choose enemy");
                case TargetType.SingleAlly:
                    return _ui.ChooseTarget(
                        _party.Select(p => p.Combatant)
                              .Where(c => c.IsAlive() && !ReferenceEquals(c, member.Combatant))
                              .ToList(),
                        "Choose ally");
                case TargetType.Self:
                    return member.Combatant;
                default:
                    return null;
            }
        }
    }
}
