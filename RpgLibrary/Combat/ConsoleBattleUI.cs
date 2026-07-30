using System;
using System.Collections.Generic;
using RpgLibrary.Contracts;

namespace RpgLibrary.Combat
{
    public class ConsoleBattleUI : IBattleUI
    {
        private int _hpBarWidth;

        public ConsoleBattleUI(int hpBarWidth = 20)
        {
            _hpBarWidth = hpBarWidth;
        }

        public virtual void ShowIntro(BattleState state)
        {
            Console.WriteLine();
            Console.WriteLine("============================================================");
            Console.WriteLine("               T U R N - B A S E D   B A T T L E            ");
            Console.WriteLine("============================================================");
            Console.WriteLine("  Every round, each combatant acts once in Speed order.");
            Console.WriteLine("  Heroes pick from Attack / Skill / Ultimate. Enemies use AI.");
            Console.WriteLine("  Battle ends when one side is wiped out.");
            Console.WriteLine("------------------------------------------------------------");
        }

        public virtual void ShowRoundStart(BattleState state)
        {
            Console.WriteLine();
            Console.WriteLine($"============  ROUND {state.Round}  ============");

            Console.WriteLine("  PARTY:");
            foreach (PartyMember p in state.Party)
            {
                ICombatant c = p.Combatant;
                string ult = "";
                if (p.Ultimate != null)
                    ult = $"  ULT[{p.Ultimate.CurrentCharge}/{p.Ultimate.MaxCharge}]";
                string ko = c.IsAlive() ? "" : "  (KO)";
                Console.WriteLine($"    - {Pad(c.Name, 12)} {HpBar(c)}{ult}{ko}");
            }

            Console.WriteLine("  ENEMIES:");
            foreach (ICombatant e in state.Enemies)
            {
                string extra = "";
                if (e is Boss b)
                {
                    extra = $"  ULT[{b.Ultimate.CurrentCharge}/{b.Ultimate.MaxCharge}]";
                    if (b.IsEnraged) extra += " *ENRAGED*";
                }
                string ko = e.IsAlive() ? "" : "  (KO)";
                Console.WriteLine($"    - {Pad(e.Name, 16)} {HpBar(e)}{extra}{ko}");
            }

            // Print turn order line: Name(Speed) -> Name(Speed) -> ...
            string order = "";
            for (int i = 0; i < state.TurnOrder.Count; i++)
            {
                ICombatant c = state.TurnOrder[i];
                if (i > 0) order += "  ->  ";
                order += $"{c.Name}({c.Speed})";
            }
            Console.WriteLine($"  Turn order: {order}");
        }

        public virtual void ShowActorTurn(ICombatant actor, bool isPartyMember)
        {
            Console.WriteLine();
            Console.WriteLine($">>> {actor.Name}'s turn  (Speed {actor.Speed})");
        }

        public virtual void ShowMessage(string message)
        {
            Console.WriteLine(message);
        }

        public virtual BattleActionOption? ChooseAction(PartyMember member, IReadOnlyList<BattleActionOption> options)
        {
            Console.WriteLine("  Choose an action:");
            for (int i = 0; i < options.Count; i++)
            {
                BattleActionOption opt = options[i];
                string tag = opt.Enabled ? "" : "  (disabled)";
                string detail = opt.Detail != null ? $"  — {opt.Detail}" : "";
                Console.WriteLine($"    [{i + 1}] {opt.Label}{detail}{tag}");
            }

            int pick = ReadNumber(1, options.Count);
            return options[pick - 1];
        }

        public virtual ICombatant? ChooseTarget(List<ICombatant> candidates, string prompt)
        {
            if (candidates.Count == 0) return null;
            if (candidates.Count == 1) return candidates[0];

            Console.WriteLine($"  {prompt}:");
            for (int i = 0; i < candidates.Count; i++)
            {
                ICombatant c = candidates[i];
                Console.WriteLine($"    [{i + 1}] {c.Name}  ({c.Health}/{c.MaxHealth} HP)");
            }

            int pick = ReadNumber(1, candidates.Count);
            return candidates[pick - 1];
        }

        public virtual Skill? ChooseSkill(List<Skill> skills)
        {
            if (skills.Count == 0) return null;

            Console.WriteLine("  Choose skill:");
            for (int i = 0; i < skills.Count; i++)
            {
                Skill s = skills[i];
                Console.WriteLine($"    [{i + 1}] {s.Name}  (power {s.Power}, targets {s.Target})");
            }

            int pick = ReadNumber(1, skills.Count);
            return skills[pick - 1];
        }

        public virtual void ShowBattleEnd(bool partyWon, int rounds)
        {
            Console.WriteLine();
            Console.WriteLine("============================================================");
            if (partyWon)
                Console.WriteLine("               *** VICTORY — party wins! ***");
            else
                Console.WriteLine("               *** DEFEAT — party wiped out. ***");
            Console.WriteLine($"                    Rounds fought: {rounds}");
            Console.WriteLine("============================================================");
        }

        // ---------- helpers ----------

        // Keep asking until the user types a number between min and max.
        protected int ReadNumber(int min, int max)
        {
            while (true)
            {
                Console.Write("  > ");
                string? line = Console.ReadLine();
                int n;
                if (int.TryParse(line, out n) && n >= min && n <= max)
                    return n;
                Console.WriteLine($"  (Please enter a number between {min} and {max}.)");
            }
        }

        // Draw an HP bar like "HP[########------------] 40/100"
        protected string HpBar(ICombatant c)
        {
            int filled = 0;
            if (c.MaxHealth > 0)
                filled = (int)Math.Round((double)c.Health / c.MaxHealth * _hpBarWidth);

            if (filled < 0) filled = 0;
            if (filled > _hpBarWidth) filled = _hpBarWidth;

            string bar = new string('#', filled) + new string('-', _hpBarWidth - filled);
            return $"HP[{bar}] {c.Health}/{c.MaxHealth}";
        }

        // Pad a name on the right so columns line up.
        protected string Pad(string s, int width)
        {
            if (s.Length >= width) return s;
            return s + new string(' ', width - s.Length);
        }
    }
}
