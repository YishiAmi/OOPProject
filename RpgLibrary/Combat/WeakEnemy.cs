namespace RpgLibrary.Combat
{
    // Low HP, low attack, fast , a scout-type enemy.
    public class WeakEnemy : Enemy
    {
        public WeakEnemy(string name = "Weak Enemy")
            : base(name, maxHealth: 40, attack: 8, defense: 2, speed: 12)
        {
            Skills.Add(new AttackSkill("Basic Attack", 10));
        }
    }
}
