namespace RpgLibrary.Contracts;

public interface ICombatant
{
    // Read-only info about the fighters
    string Name { get; }
    int Health { get; }
    int MaxHealth { get; }
    int Defense { get; }

    // Higher Speed acts earlier in a round.
    // Used by BattleSystem to build the turn order.
    int Speed { get; }

    // Behaviors every combatant must provide
    bool IsAlive();
    void TakeDamage(int damage);
    void Heal(int amount);
    void BasicAttack(ICombatant target);
}
