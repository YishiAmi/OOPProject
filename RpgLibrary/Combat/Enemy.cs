using RpgLibrary.Contracts;

namespace RpgLibrary.Combat;

public class Enemy : ICombatant
{
    public string Name { get; set; } = string.Empty;

    public int Health { get; set; }

    public int Attack { get; set; }

    public int Defense { get; set; }

    public int Speed { get; set; }

    public bool IsAlive()
    {
        return Health > 0;
    }

    public void TakeDamage(int amount)
    {
        Health = Math.Max(0, Health - Math.Max(0, amount));
    }
}
