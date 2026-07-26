using RpgLibrary.Combat;
using RpgLibrary.Items;
using RpgLibrary.World;

var character = new Character
{
    Name = "Aria",
    Level = 1,
    MaxHealth = 100,
    Health = 100,
    Attack = 12,
    Defense = 8,
    Speed = 10,
    Description = "Demo character"
};

var weapon = new Weapon
{
    Name = "Training Sword",
    Description = "A basic weapon used by the demo.",
    Price = 25,
    Damage = 5
};

var quest = new Quest
{
    Title = "First Steps",
    Objective = "Explore the library models."
};

Console.WriteLine($"{character.Name} is ready with {character.Health} health.");
Console.WriteLine($"Item: {weapon.Name} (+{weapon.Damage} damage)");
Console.WriteLine($"Quest: {quest.Title} — {quest.Objective}");
