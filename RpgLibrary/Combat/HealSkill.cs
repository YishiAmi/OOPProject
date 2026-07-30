using RpgLibrary.Contracts;

namespace RpgLibrary.Combat
{
    // Heals whoever the caller aims at. Target defaults to Self,
    // but the caller can pass SingleAlly or AllAllies to redirect.
    // Whatever Target says, that's what actually gets healed —
    // no mismatch between declared and actual target anymore.
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
