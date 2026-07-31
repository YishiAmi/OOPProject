namespace RpgLibrary.Combat
{
    // High HP, high attack
    public class StrongEnemy : Enemy
    {
        public StrongEnemy(string name = "Strong Enemy")
            : base(name, maxHealth: 90, attack: 14, defense: 8, speed: 6)
        {
            Skills.Add(new AttackSkill("Heavy Attack", 20));
        }
    }
}
