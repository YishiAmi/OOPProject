using System;
using System.Collections.Generic;
using RpgLibrary.Contracts;

namespace RpgLibrary.Combat
{
    // Shared base for all enemies. Implements ICombatant so any skill
    // can target any Enemy.
    public abstract class Enemy : ICombatant
    {
        public string Name { get; private set; }
        public int MaxHealth { get; private set; }
        public int Health { get; protected set; }
        public int Attack { get; protected set; }
        public int Defense { get; private set; }
        public int Speed { get; private set; }

        protected List<Skill> Skills { get; }

        private int _nextSkillIndex;

        protected Enemy(string name, int maxHealth, int attack, int defense, int speed = 8)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Enemy name cannot be blank.", nameof(name));
            if (maxHealth <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxHealth), "Max health must be > 0.");
            if (attack < 0)
                throw new ArgumentOutOfRangeException(nameof(attack), "Attack cannot be negative.");
            if (defense < 0)
                throw new ArgumentOutOfRangeException(nameof(defense), "Defense cannot be negative.");
            if (speed < 0)
                throw new ArgumentOutOfRangeException(nameof(speed), "Speed cannot be negative.");

            Name = name;
            MaxHealth = maxHealth;
            Health = maxHealth;
            Attack = attack;
            Defense = defense;
            Speed = speed;
            Skills = new List<Skill>();
            _nextSkillIndex = 0;
        }

        public bool IsAlive() => Health > 0;

        public virtual void TakeDamage(int damage)
        {
            if (damage < 0) damage = 0;

            int actual = damage - Defense / CombatBalance.DefenseDivisor;
            if (actual < 1) actual = 1;

            Health -= actual;
            if (Health < 0) Health = 0;

            CombatLog.Write($"  {Name} takes {actual} damage ({Health}/{MaxHealth} HP)");
        }

        public void Heal(int amount)
        {
            if (amount < 0) amount = 0;

            Health += amount;
            if (Health > MaxHealth) Health = MaxHealth;
            CombatLog.Write($"  {Name} heals {amount} ({Health}/{MaxHealth})");
        }

        // Cycle through skills each turn; fall back to a basic attack
        // if none are available. Self-targeting skills hit the caster.
        public virtual void TakeTurn(ICombatant target)
        {
            if (!IsAlive() || target == null) return;

            if (Skills.Count > 0)
            {
                Skill s = Skills[_nextSkillIndex];
                _nextSkillIndex = (_nextSkillIndex + 1) % Skills.Count;

                ICombatant actualTarget = s.Target == TargetType.Self ? this : target;
                s.Use(this, actualTarget);
            }
            else
            {
                BasicAttack(target);
            }
        }

        public void BasicAttack(ICombatant target)
        {
            if (target == null) return;
            CombatLog.Write($"{Name} swings for {Attack} damage!");
            target.TakeDamage(Attack);
        }

        public void AddSkill(Skill skill)
        {
            if (skill != null) Skills.Add(skill);
        }
    }
}
