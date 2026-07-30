using RpgLibrary.Contracts;

namespace RpgLibrary.Combat
{
    // Deals raw damage to a single enemy. The enemy's TakeDamage
    // applies defense on top, so this method just sends the raw
    // number in - no double-mitigation.
    public class AttackSkill : Skill
    {
        public override TargetType Target => TargetType.SingleEnemy;

        public AttackSkill(string name, int power) : base(name, power) { }

        public override void Use(ICombatant source, ICombatant target)
        {
            if (source == null || target == null) return;

            CombatLog.Write($"{source.Name} uses {Name} on {target.Name}!");
            target.TakeDamage(Power);
        }
    }
}
