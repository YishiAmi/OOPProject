using System;
using RpgLibrary.Contracts;

namespace RpgLibrary.Combat
{
    // A lightweight fighter for the party. Implements ICombatant so
    // any skill, ultimate, or battle system can work with it.
    //
    // Emits log lines through CombatLog (not Console.WriteLine), so a
    // WinForms consumer can capture the messages without console noise.
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
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Hero name cannot be blank.", nameof(name));
            if (maxHealth <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxHealth), "Max health must be > 0.");
            if (attack < 0)
                throw new ArgumentOutOfRangeException(nameof(attack));
            if (defense < 0)
                throw new ArgumentOutOfRangeException(nameof(defense));
            if (speed < 0)
                throw new ArgumentOutOfRangeException(nameof(speed));

            Name = name;
            MaxHealth = maxHealth;
            Health = maxHealth;
            Attack = attack;
            Defense = defense;
            Speed = speed;
        }

        public bool IsAlive() => Health > 0;

        public void TakeDamage(int damage)
        {
            if (damage < 0) damage = 0;

            int actual = damage - Defense / CombatBalance.DefenseDivisor;
            if (actual < 1) actual = 1;

            Health -= actual;
            if (Health < 0) Health = 0;

            CombatLog.Write($" {Name} takes {actual} damage ({Health}/{MaxHealth} HP)");
        }

        public void Heal(int amount)
        {
            if (amount < 0) amount = 0;

            Health += amount;
            if (Health > MaxHealth) Health = MaxHealth;
            CombatLog.Write($"  {Name} heals {amount} HP ({Health}/{MaxHealth})");
        }

        public void BasicAttack(ICombatant target)
        {
            if (target == null) return;
            CombatLog.Write($"{Name} slashes at {target.Name}!");
            target.TakeDamage(Attack);
        }
    }
}
