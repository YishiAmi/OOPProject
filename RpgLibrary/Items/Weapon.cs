namespace rpg
{
    public class Weapon
    {
        public string Name { get; set; }
        public int AttackBonus { get; set; }


        public Weapon(string name, int attackBonus)
        {
            Name = name;
            AttackBonus = attackBonus;
        }
    }
}
