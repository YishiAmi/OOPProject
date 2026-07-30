using System.Collections.Generic;

namespace RpgLibrary.Contracts
{
    // Decides what an enemy does on its turn - picks the target and
    // triggers the action. Swap it to change enemy behaviour without
    // touching BattleSystem or the Enemy classes.
    //
    // Example strategies a game dev might write:
    //   - HardestHittingStrategy : always aim at the party's fragile DPS
    //   - RunAwayStrategy        : flee when the enemy is low on HP
    //   - ScriptedBossStrategy   : follow a pre-authored move list
    public interface IEnemyStrategy
    {
        void TakeTurn(ICombatant actor,
                      List<ICombatant> livingAllies,
                      List<ICombatant> livingEnemies);
    }
}
