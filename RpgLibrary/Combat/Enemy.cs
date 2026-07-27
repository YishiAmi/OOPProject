using System;
using RpgLibrary.Contracts;
using System.Collections.Generic;

namespace RpgLibrary.Combat
{
  
    // i use abstract enemy, the shared base for all enemies.
    // Implements ICombat, so skills can target any Enemy.
    public abstract class Enemy : ICombat
    {
        public string Name {get; private set;}
        public int MaxHealth {get; private set;}
        public int Health {get; protected set;}
        public int Attack {get; protected set;}   // subclass may boost
        public int Defense {get; private set;}

        
        protected List<Skill> Skills { get; private set; }

        // Rotates through skills each turn
        private int _nextSkillIndex;

        protected Enemy(string name, int maxHealth, int attack, int defense)
        {
            Name = name;
            MaxHealth = maxHealth;
            Health = maxHealth;
            Attack = attack;
            Defense = defense;
            Skills = new List<Skill>();
            _nextSkillIndex = 0;
        }

        public bool IsAlive()
        {
            return Health > 0;
        }

        public virtual void TakeDamage(int damage)
        {
            int actual = damage - Defense / 3;
            if (actual < 1) actual = 1;

            Health -= actual;
            if (Health < 0) Health = 0;

            Console.WriteLine($"  {Name} takes {actual} damage ({Health}/{MaxHealth} HP)");
        }

        public void Heal(int amount)
        {
            Health += amount;
            if (Health > MaxHealth) Health = MaxHealth;
            Console.WriteLine($"  {Name} heals {amount} ({Health}/{MaxHealth})");
        }

        // Take a turn — cycle through skills, else basic attack.
        public virtual void TakeTurn(ICombat target)
        {
            if (!IsAlive()) return;

            if (Skills.Count > 0)
            {
                Skill s = Skills[_nextSkillIndex];
                _nextSkillIndex = (_nextSkillIndex + 1) % Skills.Count;
                s.Use(this, target);
            }
            else
            {
                Console.WriteLine($"{Name} swings for {Attack} damage!");
                target.TakeDamage(Attack);
            }
        }

        // Public entry so factories/bosses can add skills after construction.
        public void AddSkill(Skill skill)
        {
            if (skill != null) Skills.Add(skill);
        }
    }
    // WeakEnemy, low HP, low attack. can name freely
    public class WeakEnemy : Enemy
    {
        public WeakEnemy(string name = "Weak Enemy")
            : base(name, 40, 8, 2)
        {
            Skills.Add(new AttackSkill("Basic Attack", 10));
        }
    }


    // StrongEnemy, high HP, high attack
   
    public class StrongEnemy : Enemy
    {
        public StrongEnemy(string name = "Strong Enemy")
            : base(name, 90, 14, 8)
        {
            Skills.Add(new AttackSkill("Heavy Attack", 20));
        }
    }
    // HealerEnemy, attacks and can heal itself. can rename freely.

    public class HealerEnemy : Enemy
    {
        public HealerEnemy(string name = "Healer Enemy"):base(name, 55, 11, 5)
        {
            Skills.Add(new AttackSkill("Basic Attack", 12));
            Skills.Add(new HealSkill("Self Heal", 15));
        }
    }
}
