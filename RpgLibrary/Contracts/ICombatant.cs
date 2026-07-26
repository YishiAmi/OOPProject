namespace RpgLibrary.Contracts;

public interface ICombatant
{
    string Name { get; }

    int Health { get; }

    bool IsAlive();

    void TakeDamage(int amount);
}
