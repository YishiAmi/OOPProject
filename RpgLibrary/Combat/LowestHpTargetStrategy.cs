using System.Collections.Generic;
using RpgLibrary.Contracts;

namespace RpgLibrary.Combat
{
    // Focus-fire strategy: always aim at the hero with the lowest HP.
    // Great for aggressive bosses.
    public class LowestHpTargetStrategy : IEnemyStrategy
    {
        public void TakeTurn(ICombatant actor,
                             List<ICombatant> livingAllies,
                             List<ICombatant> livingEnemies)
        {
            if (actor == null || livingEnemies == null || livingEnemies.Count == 0) return;

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
