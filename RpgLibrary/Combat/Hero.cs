using System;
using RpgLibrary.Contracts;
using RPGGameLibrary.Items;

namespace RpgLibrary.Combat
{
    // The player-side fighter. Has all the RPG bits: HP, attack,
    // inventory, equipment, level, gold. Enemies use Enemy instead.
    public class Hero : ICombatant
    {
        public string Name { get; set; } = string.Empty;
        public int MaxHealth { get; set; }
        public int Health { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int Speed { get; set; }
        public string Description { get; set; } = string.Empty;

        public Weapon? EquippedWeapon { get; set; }
        public Armor? EquippedArmor { get; set; }
        public Inventory Inventory { get; set; }

        public int Level { get; set; }
        public int Experience { get; set; }
        public int ExperienceToNextLevel { get; set; } = 100;
        public int Gold { get; set; }

        // parameterless for object-initializer usage
        public Hero()
        {
            Inventory = new Inventory(20);
        }

        // convenience ctor for quick battle setups
        public Hero(string name, int maxHealth, int attack, int defense, int speed = 10)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Hero name cannot be blank.", nameof(name));
            if (maxHealth <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxHealth));

            Name = name;
            MaxHealth = maxHealth;
            Health = maxHealth;
            Attack = attack;
            Defense = defense;
            Speed = speed;
            Inventory = new Inventory(20);
        }

        public bool IsAlive() => Health > 0;

        public void TakeDamage(int damage)
        {
            if (damage < 0) damage = 0;

            int actual = damage - Defense / CombatBalance.DefenseDivisor;
            if (actual < 1) actual = 1;

            Health -= actual;
            if (Health < 0) Health = 0;

            CombatLog.Write($"  {Name} takes {actual} damage ({Health}/{MaxHealth} HP)");
        }

        public void Heal(int amount)
        {
            if (amount < 0) amount = 0;

            Health += amount;
            if (Health > MaxHealth) Health = MaxHealth;
        }

        public void BasicAttack(ICombatant target)
        {
            if (target == null) return;
            string weapon = EquippedWeapon != null ? EquippedWeapon.Name : "bare hands";
            CombatLog.Write($"{Name} strikes {target.Name} with {weapon}!");
            target.TakeDamage(Attack);
        }

        // ---- equipment ----
       public void EquipWeapon(Weapon weapon)
        {
            if (EquippedWeapon != null) Attack -= EquippedWeapon.AttackBonus;
            EquippedWeapon = weapon;
            Attack += weapon.AttackBonus;
        }

        public void EquipArmor(Armor armor)
        {
            if (EquippedArmor != null) Defense -= EquippedArmor.DefenseBonus;
            EquippedArmor = armor;
            Defense += armor.DefenseBonus;
        }

        // ---- progression ----
        public void GainExperience(int exp)
        {
            Experience += exp;

            while (Experience >= ExperienceToNextLevel)
            {
                Experience -= ExperienceToNextLevel;
                LevelUp();
                ExperienceToNextLevel += 50;
            }
        }

        public void LevelUp()
        {
            Level++;
            MaxHealth += 10;
            Health = MaxHealth;
            Attack += 2;
            Defense += 2;
            Speed += 1;
            CombatLog.Write($"{Name} level up!");
        }

        // ---- economy ----
        public void EarnGold(int amount)
        {
            if (amount > 0) Gold += amount;
        }

        public bool SpendGold(int amount)
        {
            if (amount <= 0 || Gold < amount) return false;
            Gold -= amount;
            return true;
        }
    }
}
