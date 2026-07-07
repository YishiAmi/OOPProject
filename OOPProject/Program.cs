using System;
namespace rpg
{
    public class Character
    {
        public string Name{get;set;}
        public int Level{get;set;}
        public int maxHealth{get;set;}
        public int Health{get;set;}
        public int Attack{get;set;}
        public int Defense{get;set;}
        public int Speed{get;set;}
        public string Skills{get;set;}
        public string Description{get;set;}
        public string UltimateSkill{get;set;}
        public string passiveSkill{get; set;}




        public Character(string name, int level, int maxhealth, int health, int attack, int defense, int speed, string skills, string description, string ultimateSkill, string passiveSkill)
        {
            Name = name;
            Level = level;
            maxHealth = maxhealth;
            Health = health;
            Attack = attack;
            Defense = defense;
            Speed = speed;
            Skills = skills;
            Description = description;
            UltimateSkill = ultimateSkill;
            passiveSkill = passiveSkill;
        }

    }

    public bool IsAlive()
    {
        return Health >= 0;
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
            IsDead();
        }
        
    }

    public void Heal(int amount)
    {
        Health += amount;
        if (Health > maxHealth)
        {
            Health = maxHealth;
        }
    }

    public void LevelUp()
    {
        Level++;
        maxHealth += 10;
        Health = maxHealth;
        Attack += 2;
        Defense += 2;
        Speed += 1;
    }
}

public class Enemy
{
    public string Name { get; set; }
    public int Health { get; set; }
    public int Attack { get; set; }
    public int Defense { get; set; }
    public int Speed { get; set; }
}

public class Boss
{
    public string Name { get; set; }
    public int Health { get; set; }
    public int Attack { get; set; }
    public string UltimateSkill { get; set; }
}

public class Weapon
{
    public string Name { get; set; }
    public int Damage { get; set; }
}

public class Armor
{
    public string Name { get; set; }
    public int Defense { get; set; }
}

public class Skill
{
    public string Name { get; set; }
    public int Damage { get; set; }
    public string Description { get; set; }
}

public class UltimateSkill
{
    public string Name { get; set; }
    public int Damage { get; set; }
    public int Cooldown { get; set; }
}

public class Item
{
    public string Name { get; set; }
    public string Description { get; set; }
}

public class Potion
{
    public string Name { get; set; }
    public int HealAmount { get; set; }
}

public class Inventory
{
    public int Gold { get; set; }
    public string Items { get; set; }
}

public class Quest
{
    public string Title { get; set; }
    public string Objective { get; set; }
    public bool Completed { get; set; }
}

public class NPC
{
    public string Name { get; set; }
    public string Dialogue { get; set; }
}

public class Shop
{
    public string ShopName { get; set; }
    public string ItemsAvailable { get; set; }
}

public class Map
{
    public string MapName { get; set; }
    public int Difficulty { get; set; }
}

public class GameManager
{
    public string GameStatus { get; set; }
    public int CurrentLevel { get; set; }
}
    

    
}
