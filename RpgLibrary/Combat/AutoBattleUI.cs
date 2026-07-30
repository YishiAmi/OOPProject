using System.Collections.Generic;
using RpgLibrary.Contracts;

namespace RpgLibrary.Combat
{
    // Non-interactive UI: never asks the user, picks options itself.
    // Handy for demos, CI, or an "auto-battle" mode inside a real game.
    // Inherits from ConsoleBattleUI so we still get the pretty banners,
    // HP bars, and round messages. We only override the "choose" methods.
    public class AutoBattleUI : ConsoleBattleUI
    {
        public override BattleActionOption? ChooseAction(PartyMember member, IReadOnlyList<BattleActionOption> options)
        {
            // Prefer Ultimate if it's ready, then Skill, then Attack.
            BattleActionOption? picked = FindByKind(options, BattleActionKind.Ultimate);
            if (picked == null) picked = FindByKind(options, BattleActionKind.Skill);
            if (picked == null) picked = FindByKind(options, BattleActionKind.Attack);

            if (picked != null)
                ShowMessage($"  > auto: {picked.Label}");
            return picked;
        }

        public override ICombatant? ChooseTarget(List<ICombatant> candidates, string prompt)
        {
            // Pick the target with the lowest HP - focus-fire.
            ICombatant? weakest = null;
            foreach (ICombatant c in candidates)
            {
                if (weakest == null || c.Health < weakest.Health)
                    weakest = c;
            }
            if (weakest != null)
                ShowMessage($"  > auto target: {weakest.Name}");
            return weakest;
        }

        public override Skill? ChooseSkill(List<Skill> skills)
        {
            if (skills.Count == 0) return null;
            Skill pick = skills[0];
            ShowMessage($"  > auto skill: {pick.Name}");
            return pick;
        }

        // Find the first enabled option of a given kind, or null.
        private BattleActionOption? FindByKind(IReadOnlyList<BattleActionOption> options, BattleActionKind kind)
        {
            foreach (BattleActionOption o in options)
            {
                if (o.Kind == kind && o.Enabled) return o;
            }
            return null;
        }
    }
}
