namespace RpgLibrary.Items
{
    public class Armor : Item, IEquippable
    {
        // Renamed from Defense to match Character's expected DefenseBonus contract.
        public int DefenseBonus { get; set; }
        public string ArmorType { get; set; }

        public Armor(
            string name,
            string description,
            int price,
            int defenseBonus,
            string armorType
        )
        : base(name, description, price)
        {
            DefenseBonus = defenseBonus;
            ArmorType = armorType;
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
