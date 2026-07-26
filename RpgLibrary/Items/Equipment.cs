namespace RPGGameLibrary.Items
{
    public class Equipment
    {
        public Weapon EquippedWeapon { get; private set; }

        public Armor EquippedArmor { get; private set; }


        public void EquipWeapon(Weapon weapon)
        {
            EquippedWeapon = weapon;

            Console.WriteLine(
                $"{weapon.Name} equipped"
            );
        }


        public void EquipArmor(Armor armor)
        {
            EquippedArmor = armor;

            Console.WriteLine(
                $"{armor.Name} equipped"
            );
        }


        public void ShowEquipment()
        {
            Console.WriteLine("=== Equipment ===");


            if (EquippedWeapon != null)
            {
                Console.WriteLine(
                    $"Weapon: {EquippedWeapon.Name}"
                );
            }
            else
            {
                Console.WriteLine(
                    "Weapon: None"
                );
            }


            if (EquippedArmor != null)
            {
                Console.WriteLine(
                    $"Armor: {EquippedArmor.Name}"
                );
            }
            else
            {
                Console.WriteLine(
                    "Armor: None"
                );
            }
        }
    }
}
