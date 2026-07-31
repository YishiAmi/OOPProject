namespace RpgLibrary.Combat
{
   
    public static class CombatBalance
    {
        // Defense divides incoming damage by this before subtracting.
        // Higher = defense matters less. Must be >= 1.
        public static int DefenseDivisor { get; set; } = 3;
    }
}
