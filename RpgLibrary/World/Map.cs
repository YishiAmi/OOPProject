using System.Collections.ObjectModel;
using RpgLibrary.Contracts;

namespace RpgLibrary.World;

public sealed class Map
{
    private readonly List<NPC> _npcs = new();
    private readonly List<ICombatant> _enemies = new();
    private readonly ReadOnlyCollection<NPC> _npcView;
    private readonly ReadOnlyCollection<ICombatant> _enemyView;

    public string MapName { get; }
    public int Difficulty { get; }
    public IReadOnlyList<NPC> NPCs => _npcView;
    public Shop? LocalShop { get; private set; }
    public IReadOnlyList<ICombatant> Enemies => _enemyView;

    public Map(string mapName, int difficulty)
    {
        if (string.IsNullOrWhiteSpace(mapName))
        {
            throw new ArgumentException("A map name is required.", nameof(mapName));
        }

        if (difficulty < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(difficulty),
                "Map difficulty must be at least one.");
        }

        MapName = mapName;
        Difficulty = difficulty;
        _npcView = _npcs.AsReadOnly();
        _enemyView = _enemies.AsReadOnly();
    }

    public void AddNpc(NPC npc)
    {
        ArgumentNullException.ThrowIfNull(npc);

        if (!_npcs.Exists(candidate => ReferenceEquals(candidate, npc)))
        {
            _npcs.Add(npc);
        }
    }

    public bool RemoveNpc(NPC npc)
    {
        ArgumentNullException.ThrowIfNull(npc);
        int index = _npcs.FindIndex(
            candidate => ReferenceEquals(candidate, npc));

        if (index < 0)
        {
            return false;
        }

        _npcs.RemoveAt(index);
        return true;
    }

    public bool ContainsNpc(NPC npc)
    {
        ArgumentNullException.ThrowIfNull(npc);
        return _npcs.Exists(candidate => ReferenceEquals(candidate, npc));
    }

    public void AddEnemy(ICombatant enemy)
    {
        ArgumentNullException.ThrowIfNull(enemy);

        if (!_enemies.Exists(candidate => ReferenceEquals(candidate, enemy)))
        {
            _enemies.Add(enemy);
        }
    }

    public bool RemoveEnemy(ICombatant enemy)
    {
        ArgumentNullException.ThrowIfNull(enemy);
        int index = _enemies.FindIndex(
            candidate => ReferenceEquals(candidate, enemy));

        if (index < 0)
        {
            return false;
        }

        _enemies.RemoveAt(index);
        return true;
    }

    public bool ContainsEnemy(ICombatant enemy)
    {
        ArgumentNullException.ThrowIfNull(enemy);
        return _enemies.Exists(candidate => ReferenceEquals(candidate, enemy));
    }

    public void SetLocalShop(Shop? shop)
    {
        LocalShop = shop;
    }
}
