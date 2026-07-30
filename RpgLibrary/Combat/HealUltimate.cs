using System;
using RpgLibrary.Contracts;

namespace RpgLibrary.Combat
{
    // Restores a share of the caster's max HP. Heal ratio is a ctor param so a game designer can tune it.
    public sealed class HealUltimate : UltimateSkill
    {
        public override TargetType Target => TargetType.Self;

        public double HealRatio { get; }

        public HealUltimate(string name = "Heal Ultimate",
                            int maxCharge = 3,
                            double healRatio = 1.0 / 3.0,
                            int startCharge = 3)
            : base(name, power: 0, maxCharge, damageMultiplier: 1, startCharge)
        {
            if (healRatio < 0) healRatio = 0;
            HealRatio = healRatio;
        }

        protected override void Execute(ICombatant source, ICombatant target)
        {
            int heal = (int)Math.Round(source.MaxHealth * HealRatio);
            CombatLog.Write($" *** {source.Name} uses {Name} and recovers {heal} HP ***");
            source.Heal(heal);
        }
    }
}
