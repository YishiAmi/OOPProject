using System;

namespace RpgLibrary.Combat
{
    // A thin sink domain classes (Hero, Enemy, Skill, ...) write to
    // instead of calling Console.WriteLine directly.
    //
    // The default sink is Console.WriteLine so the library still works
    // out-of-the-box. BattleSystem swaps in the current IBattleUI's
    // ShowMessage while a battle is running and restores the previous
    // sink at the end.
    //
    // Reason this exists: domain classes should not depend on the
    // console. A WinForms or Unity consumer can set their own sink
    // and never see stray Console output from the library.
    public static class CombatLog
    {
        public static Action<string> Sink { get; set; } = Console.WriteLine;

        public static void Write(string message)
        {
            Sink?.Invoke(message);
        }
    }
}
