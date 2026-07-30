using System;
using RpgLibrary.Contracts;
using RpgLibrary.Exceptions;

namespace RpgLibrary.Combat
{
    // Ultimate = a Skill that must be CHARGED before it can fire.
    // Subclasses implement Execute() with the actual effect.
    public abstract class UltimateSkill : Skill
    {
        private int _maxCharge;
        private int _currentCharge;

        public int MaxCharge => _maxCharge;
        public int CurrentCharge => _currentCharge;
        public bool IsCharged => _currentCharge >= _maxCharge;

        public int DamageMultiplier { get; set; }

        protected UltimateSkill(string name, int power, int maxCharge,
                                int damageMultiplier, int startCharge = 0)
            : base(name, power)
        {
            _maxCharge = maxCharge < 1 ? 1 : maxCharge;
            DamageMultiplier = damageMultiplier < 1 ? 1 : damageMultiplier;

            if (startCharge < 0) startCharge = 0;
            if (startCharge > _maxCharge) startCharge = _maxCharge;
            _currentCharge = startCharge;
        }

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

        public override void Use(ICombatant source, ICombatant target)
        {
            if (!IsCharged)
            {
                throw new UltimateNotChargedException(
                    $"Ultimate {Name} is not charged yet ({_currentCharge}/{_maxCharge})");
            }

            Execute(source, target);
            _currentCharge = 0;
        }

        protected abstract void Execute(ICombatant source, ICombatant target);

        public override string ToString()
        {
            return $"[ULT] {Name} (charge {_currentCharge}/{_maxCharge}, x{DamageMultiplier})";
        }
    }
}
