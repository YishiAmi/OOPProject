using System.Collections.Generic;
using RpgLibrary.Combat;

namespace RpgLibrary.Contracts
{
    // BattleSystem talks to the outside world through this interface.
    //   - Console apps -> ConsoleBattleUI (menus via Console.ReadLine)
    //   - Auto-play    -> AutoBattleUI (picks the best option itself)
    //   - Unity / WPF  -> your own class that implements this interface
    //
    // BattleSystem itself has NO console code. That is why swapping
    // the UI does not need a single change inside BattleSystem.
    public interface IBattleUI
    {
        // Once at the start of the battle.
        void ShowIntro(BattleState state);

        // Once at the start of every round, with a fresh state snapshot.
        void ShowRoundStart(BattleState state);

        // Right before a combatant takes their action.
        void ShowActorTurn(ICombatant actor, bool isPartyMember);

        // A plain log line — attacks landed, exceptions caught, etc.
        void ShowMessage(string message);

        // Ask which action to take. Return one of the options (or null
        // if none are pickable, in which case the turn is skipped).
        BattleActionOption? ChooseAction(PartyMember member, IReadOnlyList<BattleActionOption> options);

        // Ask which target to hit. Candidates are already filtered to
        // living combatants.
        ICombatant? ChooseTarget(List<ICombatant> candidates, string prompt);

        // Ask which skill to use.
        Skill? ChooseSkill(List<Skill> skills);

        // Once at the end.
        void ShowBattleEnd(bool partyWon, int rounds);
    }
}
