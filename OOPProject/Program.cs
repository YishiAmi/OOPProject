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
    // Dummy Classes

public class Weapon
{
    public string Name { get; set; }
    public int Damage { get; set; }

    public Weapon(string name, int damage)
    {
        Name = name;
        Damage = damage;
    }
}

public class Armor
{
    public string Name { get; set; }
    public int Defense { get; set; }

    public Armor(string name, int defense)
    {
        Name = name;
        Defense = defense;
    }
}

public class Item
{
    public string Name { get; set; }
    public string Description { get; set; }

    public Item(string name, string description)
    {
        Name = name;
        Description = description;
    }
}

public class Enemy
{
    public string Name { get; set; }
    public int Health { get; set; }
    public int Attack { get; set; }

    public Enemy(string name, int health, int attack)
    {
        Name = name;
        Health = health;
        Attack = attack;
    }
}

public class Quest
{
    public string Title { get; set; }
    public string Objective { get; set; }
    public bool IsCompleted { get; set; }

    public Quest(string title, string objective)
    {
        Title = title;
        Objective = objective;
        IsCompleted = false;
    }
}

public class Inventory
{
    public int Gold { get; set; }

    public Inventory(int gold)
    {
        Gold = gold;
    }
}

    

    
}
