using System.Collections.Generic;
using RpgLibrary.Contracts;

namespace RpgLibrary.Combat
{
    public enum BattleActionKind
    {
        Attack,
        Skill,
        Ultimate,
        Defend,
        Flee
    }
    public class BattleActionOption
    {
        public BattleActionKind Kind {get;}
        public string Label {get;}
        public bool Enabled {get;}
        public string? Detail {get;}   // e.g. "3/4 charged", "0 potions left"

        public BattleActionOption(BattleActionKind kind, string label, bool enabled = true, string? detail = null)
        {
            Kind = kind;
            Label = label;
            Enabled = enabled;
            Detail = detail;
        }
    }

    public class BattleState
    {
        public int Round {get;}
        public IReadOnlyList<PartyMember> Party {get;}
        public IReadOnlyList<ICombatant> Enemies {get;}
        public IReadOnlyList<ICombatant> TurnOrder {get;}

        public BattleState(int round, IReadOnlyList<PartyMember> party, IReadOnlyList<ICombatant> enemies, IReadOnlyList<ICombatant> turnOrder)  
        {
            Round = round;
            Party = party;
            Enemies = enemies;
            TurnOrder = turnOrder;
        }
    }
}
