namespace RPGGameLibrary.Items
{
    public class Weapon : Item
    {
        public int Damage { get; set; }

        public string WeaponType { get; set; }


        public Weapon(
            string name,
            string description,
            int value,
            int damage,
            string weaponType
        )
        : base(name, description, value)
        {
            Damage = damage;
            WeaponType = weaponType;
        }


        public override void Use()
        {
            Console.WriteLine(
                $"Equipped {Name}"
            );

            Console.WriteLine(
                $"Damage +{Damage}"
            );
        }
    }
}
