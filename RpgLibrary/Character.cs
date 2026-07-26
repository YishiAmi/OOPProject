using System;

namespace rpg
{
    public class Character
    {
        public string Name { get; set; }
        public int Level { get; set; }
        public int MaxHealth { get; set; }
        public int Health { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int Speed { get; set; }

        public string Skills { get; set; }
        public string Description { get; set; }
        public string UltimateSkill { get; set; }
        public string PassiveSkill { get; set; }

        public Weapon EquippedWeapon { get; set; }
        public Armor EquippedArmor { get; set; }

        public Inventory Inventory { get; set; }

        public int Experience { get; set; }
        public int ExperienceToNextLevel { get; set; } = 100;

        public int Gold { get; set; }


        public Character(
            string name,
            int level,
            int maxHealth,
            int health,
            int attack,
            int defense,
            int speed,
            string skills,
            string description,
            string ultimateSkill,
            string passiveSkill)
        {
            Name = name;
            Level = level;
            MaxHealth = maxHealth;
            Health = health;
            Attack = attack;
            Defense = defense;
            Speed = speed;

            Skills = skills;
            Description = description;
            UltimateSkill = ultimateSkill;
            PassiveSkill = passiveSkill;

            Inventory = new Inventory();

            Experience = 0;
            Gold = 0;
        }


        public bool IsAlive()
        {
            return Health > 0;
        }


        public bool IsDead()
        {
            return Health <= 0;
        }


        public void TakeDamage(int damage)
        {
            Health -= damage;

            if (Health < 0)
            {
                Health = 0;
            }
        }


        public void Heal(int amount)
        {
            Health += amount;

            if (Health > MaxHealth)
            {
                Health = MaxHealth;
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

            Console.WriteLine(Name + " level up!");
        }


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


        public void EquipWeapon(Weapon weapon)
        {
            if (EquippedWeapon != null)
            {
                Attack -= EquippedWeapon.AttackBonus;
            }

            EquippedWeapon = weapon;
            Attack += weapon.AttackBonus;
        }


        public void EquipArmor(Armor armor)
        {
            if (EquippedArmor != null)
            {
                Defense -= EquippedArmor.DefenseBonus;
            }

            EquippedArmor = armor;
            Defense += armor.DefenseBonus;
        }


        public void EarnGold(int amount)
        {
            Gold += amount;
        }


        public void SpendGold(int amount)
        {
            if (Gold >= amount)
            {
                Gold -= amount;
            }
        }
    }
}
