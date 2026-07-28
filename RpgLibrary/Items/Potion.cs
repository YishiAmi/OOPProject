namespace RPGGameLibrary.Items
{
    public class Potion : Item
    {
        public int HealAmount { get; set; }
        public Potion(
            string name,
            string description,
            int price,
            int healAmount
        )
        : base(name, description, price)
        {
            HealAmount = healAmount;
        }
        public void Drink()
        {
            Console.WriteLine(
                $"{Name} restored {HealAmount} HP"
            );
        }
        public override void Use()
        {
            Drink();
        }
    }
}
