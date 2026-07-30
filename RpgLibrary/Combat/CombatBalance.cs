namespace RpgLibrary.Combat
{
    // Central home for game-wide balance numbers so they aren't
    // scattered as magic numbers across the codebase. A game designer
    // can retune here without editing individual class files.
    // Everything is settable at runtime - the game host (Program.cs) can read a config file and set these before the first battle.
    public static class CombatBalance
    {
        // Defense divides incoming damage by this before subtracting.
        // Higher = defense matters less. Must be >= 1.
        public static int DefenseDivisor { get; set; } = 3;
    }
}
