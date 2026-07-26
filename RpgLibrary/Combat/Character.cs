using RpgLibrary.Contracts;

namespace RpgLibrary.Combat;

public class Character : ICombatant
{
    public string Name { get; set; } = string.Empty;

    public int Level { get; set; } = 1;

    public int MaxHealth { get; set; }

    public int Health { get; set; }

    public int Attack { get; set; }

    public int Defense { get; set; }

    public int Speed { get; set; }

    public string Description { get; set; } = string.Empty;

    public bool IsAlive()
    {
        return Health > 0;
    }

    public void TakeDamage(int amount)
    {
        Health = Math.Max(0, Health - Math.Max(0, amount));
    }

    public void Heal(int amount)
    {
        Health = Math.Min(MaxHealth, Health + Math.Max(0, amount));
    }
}
