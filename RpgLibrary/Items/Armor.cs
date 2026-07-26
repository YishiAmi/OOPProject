namespace RPGGameLibrary.Items
{
    public class Armor : Item
    {
        public int Defense { get; set; }

        public string ArmorType { get; set; }


        public Armor(
            string name,
            string description,
            int value,
            int defense,
            string armorType
        )
        : base(name, description, value)
        {
            Defense = defense;
            ArmorType = armorType;
        }


        public override void Use()
        {
            Console.WriteLine(
                $"Equipped {Name}"
            );

            Console.WriteLine(
                $"Defense +{Defense}"
            );
        }
    }
}
