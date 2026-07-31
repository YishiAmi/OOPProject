namespace RPGGameLibrary.Items
{
    public class Weapon : Item, IEquippable
    {
        // Renamed from Damage to match Character's expected AttackBonus contract.
        public int AttackBonus { get; set; }
        public string WeaponType { get; set; }

        public Weapon(
            string name,
            string description,
            int price,
            int attackBonus,
            string weaponType
        )
        : base(name, description, price)
        {
            AttackBonus = attackBonus;
            WeaponType = weaponType;
        }

        public override void Use()
        {
            Equip();
        }

        public void Equip()
        {
            // Intentionally no console output - Items is a lower-level
            // library that Combat depends on, so it can't use CombatLog
            // without creating a circular dependency. Consumers (Hero,
            // Enemy, UI layer) narrate equip events themselves if needed.
        }

        public void Unequip()
        {
        }
    }
}
