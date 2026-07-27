using System;

namespace rpg
{
    public interface ICombat
    {
        // Read-only info about the fighter
        string Name { get; }
        int Health { get; }
        int MaxHealth { get; }
        int Defense { get; }

        // things that the fighters needs to have in their code
        bool IsAlive();
        void TakeDamage(int damage);
        void Heal(int amount);
    }
}
