using System;
using RpgLibrary.Contracts;
namespace RpgLibrary.Combat
{
    // Hero , a simple class that implements ICombatant.

    public class Hero : ICombatant
    {
     
        public string Name { get; private set; }
        public int MaxHealth { get; private set; }
        public int Health { get; private set; }
        public int Attack { get; private set; }
        public int Defense { get; private set; }
        public int Speed { get; private set; }

        public Hero(string name, int maxHealth, int attack, int defense, int speed = 10)
        {
            Name = name;
            MaxHealth = maxHealth;
            Health = maxHealth;   // start at full HP
            Attack = attack;
            Defense = defense;
            Speed = speed;
        }

        public bool IsAlive()
        {
            return Health > 0;
        }

        public void TakeDamage(int damage)
        {
            // Defense reduces incoming damage a bit
            int actual = damage - Defense / 3;
            if (actual < 1) actual = 1;

            Health -= actual;
            if (Health < 0) Health = 0;

            Console.WriteLine($" {Name} takes {actual} damage ({Health}/{MaxHealth} HP)");
        }

        public void Heal(int amount)
        {
            Health += amount;
            if (Health > MaxHealth) Health = MaxHealth;
            Console.WriteLine($"  {Name} heals {amount} HP ({Health}/{MaxHealth})");
        }

    
        public void BasicAttack(ICombatant target)
        {
            Console.WriteLine($"{Name} slashes at {target.Name}!");
            target.TakeDamage(Attack);
        }
    }
}
