using RpgLibrary.Contracts;

namespace RpgLibrary.Combat
{
    // Heavy-damage ultimate. Deals base * multiplier and lets the
    // target's TakeDamage apply defense. No manual defense math here
    public sealed class DamageUltimate : UltimateSkill
    {
        public override TargetType Target => TargetType.SingleEnemy;

        public int BaseDamage {get;}

        public DamageUltimate(string name = "Damage Ultimate",
                              int baseDamage = 40,
                              int maxCharge = 3,
                              int damageMultiplier = 3,
                              int startCharge = 3)
            : base(name, baseDamage, maxCharge, damageMultiplier, startCharge)
        {
            BaseDamage = baseDamage < 1 ? 1 : baseDamage;
        }

        protected override void Execute(ICombatant source, ICombatant target)
        {
            int dmg = BaseDamage * DamageMultiplier;
            CombatLog.Write($"  *** {source.Name} unleashes {Name} on {target.Name} for {dmg} raw damage! ***");
            target.TakeDamage(dmg);
        }
    }
}
