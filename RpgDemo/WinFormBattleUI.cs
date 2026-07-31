using System.Collections.Generic;
using System.Threading;
using RpgLibrary.Combat;
using RpgLibrary.Contracts;

namespace RpgDemo
{
    // Plugs BattleSystem into BattleForm. BattleSystem runs on a background
    // thread, this class marshals its calls back onto the UI thread and
    // blocks until the player clicks a button.
    public class WinFormBattleUI : IBattleUI
    {
        private readonly BattleForm _form;
        private readonly AutoResetEvent _wait = new AutoResetEvent(false);

        private BattleActionOption? _chosenAction;
        private ICombatant? _chosenTarget;
        private Skill? _chosenSkill;

        public WinFormBattleUI(BattleForm form)
        {
            _form = form;
        }

        public void ShowIntro(BattleState state)
        {
            _form.UiAppendLog("========== TURN-BASED BATTLE ==========");
            _form.UiRefreshState(state);
        }

        public void ShowRoundStart(BattleState state)
        {
            _form.UiAppendLog($"\n===== Round {state.Round} =====");
            _form.UiRefreshState(state);
        }

        public void ShowActorTurn(ICombatant actor, bool isPartyMember)
        {
            _form.UiShowTurn(actor, isPartyMember);
        }

        public void ShowMessage(string message)
        {
            _form.UiAppendLog(message);
        }

        public void ShowBattleEnd(BattleOutcome outcome, int rounds)
        {
            string tag = outcome switch
            {
                BattleOutcome.Victory => $"\n*** VICTORY - party wins in {rounds} rounds ***",
                BattleOutcome.Defeat  => $"\n*** DEFEAT - party wiped out after {rounds} rounds ***",
                BattleOutcome.Timeout => $"\n*** TIMEOUT - round limit reached ({rounds} rounds) ***",
                _                     => "\n*** UNDECIDED ***",
            };
            _form.UiAppendLog(tag);
            _form.UiShowEnd(outcome, rounds);
        }

        public BattleActionOption? ChooseAction(PartyMember member, IReadOnlyList<BattleActionOption> options)
        {
            _chosenAction = null;
            _form.UiPromptAction(member, options, choice =>
            {
                _chosenAction = choice;
                _wait.Set();
            });
            _wait.WaitOne();
            return _chosenAction;
        }

        public ICombatant? ChooseTarget(List<ICombatant> candidates, string prompt)
        {
            _chosenTarget = null;
            _form.UiPromptTarget(candidates, prompt, choice =>
            {
                _chosenTarget = choice;
                _wait.Set();
            });
            _wait.WaitOne();
            return _chosenTarget;
        }

        public Skill? ChooseSkill(List<Skill> skills)
        {
            _chosenSkill = null;
            _form.UiPromptSkill(skills, choice =>
            {
                _chosenSkill = choice;
                _wait.Set();
            });
            _wait.WaitOne();
            return _chosenSkill;
        }
    }
}
