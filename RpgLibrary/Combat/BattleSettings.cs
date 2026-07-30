namespace RpgLibrary.Combat
{
   
    public class BattleSettings
    {
        // How much charge each party-member ultimate gains just from acting.
        // Enemies charge via TakeDamage (see Boss.TakeDamage).
        public int UltimateChargePerTurn {get; set;} = 1;

        // 0 = unlimited. Otherwise the battle ends in a draw after this
        // many rounds. Useful for boss puzzles or timed encounters.
        public int MaxRounds { get; set; } = 0;

        // If true, KO'd party members are removed from the turn order
        // and cannot act until revived. If false, they remain in the turn order
        // and will be skipped when their turn comes up.
        public bool RemoveKOFromTurnOrder { get; set; } = true;
    }
}
