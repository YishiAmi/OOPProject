using System;
using RpgLibrary.Contracts;

namespace RpgLibrary.Combat
{
    // Base class for every combat skill. Concrete subclasses define
    // how the skill applies its effect and who it targets.
    public abstract class Skill
    {
        public string Name { get; set; }
        public int Power { get; set; }

        // Every skill declares what it targets. BattleSystem uses this
        // to pick the right prompt (self / ally / enemy / all).
        // No downcasting anywhere.
        public abstract TargetType Target { get; }

        protected Skill(string name, int power)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Skill name cannot be blank.", nameof(name));
            if (power < 0)
                throw new ArgumentOutOfRangeException(nameof(power), "Power cannot be negative.");

            Name = name;
            Power = power;
        }

        // source = who is using the skill; target = who is receiving it.
        public abstract void Use(ICombatant source, ICombatant target);
    }
}
