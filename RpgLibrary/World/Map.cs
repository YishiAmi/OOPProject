using System.Collections.Generic;
using RpgLibrary.Contracts;

namespace RpgLibrary.World
{
    public class Map
    {
        // basic stuff 
        public string MapName { get; }
        public int Difficulty { get; }
        
        // awsome intgrations
        public List<NPC> NPCs { get; } = new();
        public Shop? LocalShop { get; set; }
        public List<ICombatant> Enemies { get; } = new();

        public Map(string mapName, int difficulty)
        {
            MapName = mapName;
            Difficulty = difficulty;
        }    
    }
}

/*
    Notes:

    -   

*/