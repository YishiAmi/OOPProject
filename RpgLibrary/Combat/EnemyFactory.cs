using System;

namespace RpgLibrary.Combat
{

    
    // All names are just placeholders, pass a custom name to override.

    public static class EnemyFactory
    {
        public static Enemy CreateWeakEnemy(string name = "Weak Enemy")
        {
            return new WeakEnemy(name);
        }

        public static Enemy CreateStrongEnemy(string name = "Strong Enemy")
        {
            return new StrongEnemy(name);
        }

        public static Enemy CreateHealerEnemy(string name = "Healer Enemy")
        {
            return new HealerEnemy(name);
        }

        // Boss with the damage-type ultimate
        public static Boss CreateDamageBoss(string name = "Damage Boss")
        {
            Boss boss = new Boss(name, 180, 15, 10, new DamageUltimate());
            boss.AddSkill(new AttackSkill("Boss Attack 1", 18));
            boss.AddSkill(new AttackSkill("Boss Attack 2", 25));
            return boss;
        }

        // Boss with the drain-type ultimate
        public static Boss CreateDrainBoss(string name = "Drain Boss")
        {
            Boss boss = new Boss(name, 200, 14, 14, new DrainUltimate());
            boss.AddSkill(new AttackSkill("Boss Attack", 16));
            boss.AddSkill(new HealSkill("Boss Heal", 25));
            return boss;
        }
    }
}
