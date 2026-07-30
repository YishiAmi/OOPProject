namespace RpgLibrary.Combat
{
   
    public class BattleSettings
    {
        public int UltimateChargePerTurn {get; set;} = 1;
        public int MaxRounds { get; set; } = 0;
        public bool RemoveKOFromTurnOrder { get; set; } = true;
    }
}
