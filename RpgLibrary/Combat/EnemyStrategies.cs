using System;
using System.Collections.Generic;
using RpgLibrary.Contracts;

namespace RpgLibrary.Combat
{
    // Default strategy: pick a random living hero as the target.
    public class RandomTargetStrategy : IEnemyStrategy
    {
        private Random _rng;

        public RandomTargetStrategy()
        {
            _rng = new Random();
        }

        public RandomTargetStrategy(Random rng)
        {
            _rng = rng;
        }

        public void TakeTurn(ICombatant actor,
                             List<ICombatant> livingAllies,
                             List<ICombatant> livingEnemies)
        {
            if (livingEnemies.Count == 0) return;

            int index = _rng.Next(livingEnemies.Count);
            ICombatant target = livingEnemies[index];

            // If the actor is an Enemy, use its built-in skill rotation.
            // Otherwise fall back to a basic attack.
            if (actor is Enemy e) e.TakeTurn(target);
            else actor.BasicAttack(target);
        }
    }

    // Focus-fire strategy: always aim at the hero with the lowest HP.
    // Great for aggressive bosses.
    public class LowestHpTargetStrategy : IEnemyStrategy
    {
        public void TakeTurn(ICombatant actor,
                             List<ICombatant> livingAllies,
                             List<ICombatant> livingEnemies)
        {
            if (livingEnemies.Count == 0) return;

            // Find the target with the smallest Health.
            ICombatant target = livingEnemies[0];
            for (int i = 1; i < livingEnemies.Count; i++)
            {
                if (livingEnemies[i].Health < target.Health)
                    target = livingEnemies[i];
            }

            if (actor is Enemy e) e.TakeTurn(target);
            else actor.BasicAttack(target);
        }
    }
}
