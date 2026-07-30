using System.Collections.Generic;
using RpgLibrary.Combat;

namespace RpgLibrary.Contracts
{
    // BattleSystem talks to the outside world through this interface.
    //   - Console apps -> ConsoleBattleUI
    //   - Auto-play    -> AutoBattleUI
    //   - Unity / WPF  -> your own class that implements this interface
    //
    // BattleSystem itself has NO console code. Swapping the UI does
    // not require a single change inside BattleSystem.
    public interface IBattleUI
    {
        void ShowIntro(BattleState state);
        void ShowRoundStart(BattleState state);
        void ShowActorTurn(ICombatant actor, bool isPartyMember);
        void ShowMessage(string message);

        BattleActionOption? ChooseAction(PartyMember member, IReadOnlyList<BattleActionOption> options);
        ICombatant? ChooseTarget(List<ICombatant> candidates, string prompt);
        Skill? ChooseSkill(List<Skill> skills);

        void ShowBattleEnd(BattleOutcome outcome, int rounds);
    }
}
