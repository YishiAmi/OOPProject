using System;
using System.Collections.Generic;
using RpgLibrary.Contracts;

namespace RpgLibrary.Combat
{
    // Default strategy: pick a random living hero and let Enemy.TakeTurn
    // run its built-in skill rotation (or basic attack if no skills).
    public class RandomTargetStrategy : IEnemyStrategy
    {
        private readonly Random _rng;

        public RandomTargetStrategy() { _rng = new Random(); }
        public RandomTargetStrategy(Random rng) { _rng = rng ?? new Random(); }

        public void TakeTurn(ICombatant actor,
                             List<ICombatant> livingAllies,
                             List<ICombatant> livingEnemies)
        {
            if (actor == null || livingEnemies == null || livingEnemies.Count == 0) return;

            ICombatant target = livingEnemies[_rng.Next(livingEnemies.Count)];

            if (actor is Enemy e) e.TakeTurn(target);
            else actor.BasicAttack(target);
        }
    }
}
