namespace RpgLibrary.Combat
{
    // Attacks and can heal itself. Skill.Target = Self makes the
    // heal actually apply to the caster, not the player.
    public class HealerEnemy : Enemy
    {
        public HealerEnemy(string name = "Healer Enemy")
            : base(name, maxHealth: 55, attack: 11, defense: 5, speed: 9)
        {
            Skills.Add(new AttackSkill("Basic Attack", 12));
            Skills.Add(new HealSkill("Self Heal", 15));
        }
    }
}
