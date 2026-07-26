namespace rpg
{
    public class Potion : Item
    {
        public int HealAmount { get; set; }


        public Potion(string name, string description, int healAmount)
            : base(name, description)
        {
            HealAmount = healAmount;
        }


        public void Use(Character character)
        {
            character.Heal(HealAmount);
        }
    }
}
