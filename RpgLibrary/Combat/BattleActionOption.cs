namespace RpgLibrary.Combat
{
    // One button on the action menu. BattleSystem builds the list;
    // the UI just shows it and returns the one the player picked.
    public class BattleActionOption
    {
        public BattleActionKind Kind {get;}
        public string Label {get;}
        public bool Enabled {get;}
        public string? Detail {get;}

        public BattleActionOption(BattleActionKind kind, string label, bool enabled = true, string? detail = null)
        {
            Kind = kind;
            Label = label ?? string.Empty;
            Enabled = enabled;
            Detail = detail;
        }
    }
}
