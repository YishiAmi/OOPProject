namespace RpgLibrary.Contracts;

public interface ICombat
{
    // Read-only info about the fighters
    string Name { get; }
    int Health { get; }
    int MaxHealth { get; }
    int Defense { get; }

    // Behaviors every combatant must provide
    bool IsAlive();
    void TakeDamage(int damage);
    void Heal(int amount);
}
