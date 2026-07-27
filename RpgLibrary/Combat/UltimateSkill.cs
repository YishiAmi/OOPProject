using System;

namespace RpgLibrary.Combat
{

    // Ultimate = a Skill that must be CHARGED before it can fire.
    // Extends Skill and overrides the abstract Use() method.
    public abstract class UltimateSkill : Skill
    {
        private int _maxCharge {get; set;}
        private int _currentCharge {get; set;}

        public int MaxCharge
        {
            get { return _maxCharge; }
        }

        public int CurrentCharge
        {
            get { return _currentCharge; }
        }

        public bool IsCharged
        {
            get { return _currentCharge >= _maxCharge; }
        }

        public int DamageMultiplier {get;set;}


        protected UltimateSkill(string name, int power, int maxCharge, int damageMultiplier): base(name, power)
        {
            if (maxCharge < 1)
            {
                _maxCharge = 1;
            }
            else
            {
                _maxCharge = maxCharge;
            }

            _currentCharge = 0;

            if (damageMultiplier < 1)
            {
                DamageMultiplier = 1;
            }
            else
            {
                DamageMultiplier = damageMultiplier;
            }
        }

        // Add charge, typically called when the owner takes damage or attacks
        public void Charge(int amount = 1)
        {
            if (amount < 0) amount = 0;
            _currentCharge += amount;
            if (_currentCharge > _maxCharge) _currentCharge = _maxCharge;
        }

        public void ResetCharge()
        {
            _currentCharge = 0;
        }

        public override void Use(ICombat source, ICombat target)
        {
            if (!IsCharged)
            {
                Console.WriteLine($" {Name} not fully charged ({_currentCharge}/{_maxCharge}).");
                return;
            }
            Execute(source, target);
            _currentCharge = 0;   // consume the charge
        }

        
        protected abstract void Execute(ICombat source, ICombat target);

        public override string ToString()
        {
            return $"[ULT] {Name} (charge {_currentCharge}/{_maxCharge}, x{DamageMultiplier})";
        }
    }


    // Heavy-damage ultimate , deals big damage reduced by defense.
    public sealed class DamageUltimate : UltimateSkill
    {
        public int BaseDamage { get; private set; }

        public DamageUltimate(string name = "Damage Ultimate", int baseDamage = 40): base(name, baseDamage, maxCharge: 3, damageMultiplier: 3)
        {
            if (baseDamage < 1)
                BaseDamage = 1;
            else
                BaseDamage = baseDamage;
        }

        protected override void Execute(ICombat source, ICombat target)
        {
            int dmg = (BaseDamage * DamageMultiplier) - target.Defense;
            if (dmg < 1) dmg = 1;

            Console.WriteLine($"  *** {source.Name} unleashes {Name} on {target.Name} for {dmg} damage! ***");
            target.TakeDamage(dmg);
        }
    }

    // Drain ultimate , deals damage ignoring defense and heals caster for 25% of it.
    public sealed class DrainUltimate : UltimateSkill
    {
        public DrainUltimate(string name = "Drain Ultimate"): base(name, 35, maxCharge: 4, damageMultiplier: 2)
        {
        }

        protected override void Execute(ICombat source, ICombat target)
        {
            int dmg = Power * DamageMultiplier;   // ignores defense
            int drained = dmg / 4;                // 25% back as healing

            Console.WriteLine($"  *** {source.Name} uses {Name} on {target.Name} for {dmg} damage (ignores defense) ***");
            target.TakeDamage(dmg);

            Console.WriteLine($"  *** {source.Name} recovers {drained} HP from {Name} ***");
            source.Heal(drained);
        }
    }

    // Heal ultimate — restores 1/3 of the caster's max HP.
    public sealed class HealUltimate : UltimateSkill
    {
        public HealUltimate(string name = "Heal Ultimate"): base(name, 0, maxCharge: 3, damageMultiplier: 2)
        {
        }

        protected override void Execute(ICombat source, ICombat target)
        {
            int heal = source.MaxHealth / 3;
            Console.WriteLine($" *** {source.Name} uses {Name} and recovers {heal} HP ***");
            source.Heal(heal);
        }
    }
}
