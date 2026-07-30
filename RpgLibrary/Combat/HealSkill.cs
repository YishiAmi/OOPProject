using RpgLibrary.Contracts;

namespace RpgLibrary.Combat
{
    public class HealSkill : Skill
    {
        public override TargetType Target { get; }

        public HealSkill(string name, int healAmount, TargetType target = TargetType.Self)
            : base(name, healAmount)
        {
            Target = target;
        }

        public override void Use(ICombatant source, ICombatant target)
        {
            if (source == null || target == null) return;

            CombatLog.Write($"{source.Name} heals {target.Name} with {Name} for {Power} HP");
            target.Heal(Power);
        }
    }
}
