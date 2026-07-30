using System;
using RpgLibrary.Contracts;

namespace RpgLibrary.Combat
{
    // Deals damage that TRULY ignores defense (bypasses TakeDamage's
    // mitigation) and heals the caster for a share of the damage dealt.
    // Drain ratio is a ctor param so a game designer can tune it.
    public sealed class DrainUltimate : UltimateSkill
    {
        public override TargetType Target => TargetType.SingleEnemy;

        public double DrainRatio { get; }

        public DrainUltimate(string name = "Drain Ultimate",
                             int power = 35,
                             int maxCharge = 4,
                             int damageMultiplier = 2,
                             double drainRatio = 0.25,
                             int startCharge = 3)
            : base(name, power, maxCharge, damageMultiplier, startCharge)
        {
            if (drainRatio < 0) drainRatio = 0;
            DrainRatio = drainRatio;
        }

        protected override void Execute(ICombatant source, ICombatant target)
        {
            int dmg = Power * DamageMultiplier;
            int drained = (int)Math.Round(dmg * DrainRatio);

            // Add back the defense the target's TakeDamage would remove,
            // so the net HP loss equals the full 'dmg' — truly ignoring defense.
            int compensated = dmg + target.Defense / CombatBalance.DefenseDivisor;

            CombatLog.Write($"  *** {source.Name} uses {Name} on {target.Name} for {dmg} damage (ignores defense) ***");
            target.TakeDamage(compensated);

            CombatLog.Write($"  *** {source.Name} recovers {drained} HP from {Name} ***");
            source.Heal(drained);
        }
    }
}
