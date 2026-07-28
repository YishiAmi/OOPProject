using System;
using RpgLibrary.Contracts;

namespace RpgLibrary.World
{
    public class GameManager
    {
        public ICombatant Player { get; }
        public Map CurrentMap { get; private set; }
        public int PlayerGold { get; private set; } 

        public GameManager(ICombatant player, Map startingMap)
        {
            Player = player;
            CurrentMap = startingMap;
            PlayerGold = 100; // Starting gold for testing
        }

        public void ChangeMap(Map newMap)
        {
            CurrentMap = newMap;
            Console.WriteLine($"Traveling to {CurrentMap.MapName} (Difficulty: {CurrentMap.Difficulty})...");
        }

        public void TalkToNPC(NPC npc)
        {
            Console.WriteLine($"[{npc.Name}]: {npc.Talk()}");
            
            if (npc.OfferedQuest != null && !npc.OfferedQuest.Completed)
            {
                Console.WriteLine($"--> New Quest Acquired: {npc.OfferedQuest.Title} ({npc.OfferedQuest.Objective})");
            }
        }

        public void CompleteQuest(Quest quest)
        {
            if (!quest.Completed)
            {
                quest.Complete(); // Triggers the virtual/override logic we discussed
                PlayerGold += quest.GoldReward;
                Console.WriteLine($"Quest '{quest.Title}' completed! Earned {quest.GoldReward} gold.");
            }
            else
            {
                Console.WriteLine($"Quest '{quest.Title}' is already completed.");
            }
        }

        public void EnterShop(Shop shop, IShopItem itemToBuy)
        {
            Console.WriteLine($"Entering {shop.ShopName}...");
            int currentGold = PlayerGold;
            
            if (shop.Buy(itemToBuy, ref currentGold))
            {
                PlayerGold = currentGold;
                Console.WriteLine($"Successfully bought {itemToBuy.Name}. Remaining Gold: {PlayerGold}");
            }
            else
            {
                Console.WriteLine($"Failed to buy {itemToBuy.Name}. Not enough gold or item unavailable.");
            }
        }

        public void Attack(ICombatant target)
        {
            Console.WriteLine($"{Player.Name} attacks {target.Name}!");
            
            // Change this on the game later
            target.TakeDamage(15); 
            
            if (!target.IsAlive())
            {
                Console.WriteLine($"{target.Name} has been defeated!");
            }
            else
            {
                Console.WriteLine($"{target.Name} has {target.Health} HP remaining.");
            }
        }
    }
}

/*
    Notes:

    -   the game manger can act as a console theoritcall the same way you'd control everything in 
        a minecraft server but is that how you want it to be or something to be used by main game demo
        [update: if you use the latter then you essentially implement a facade class, two birds one stone]

    -   your idea for the facade is to abstract as many stuff as you can so long it makes sense
    -   methods planned ChangeMap, EnterShop, Attack, TalkToNPC, CompleteQuest
    -   keep in mind you can also do the same for shop

*/