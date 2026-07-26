namespace rpg
{
    public class Armor
    {
        public string Name { get; set; }
        public int DefenseBonus { get; set; }


        public Armor(string name, int defenseBonus)
        {
            Name = name;
            DefenseBonus = defenseBonus;
        }
    }
}
