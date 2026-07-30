using System.Collections.Generic;
using RpgLibrary.Contracts;

namespace RpgLibrary.Combat
{
    // A snapshot of the battlefield handed to the UI so it can render.
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
