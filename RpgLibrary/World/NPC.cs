namespace RpgLibrary.World;

public sealed class NPC
{
    public string Name { get; }
    public string Dialogue { get; }
    public Quest? OfferedQuest { get; private set; }

    public NPC(string name, string dialogue, Quest? offeredQuest = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("An NPC name is required.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(dialogue))
        {
            throw new ArgumentException(
                "NPC dialogue cannot be empty.",
                nameof(dialogue));
        }

        Name = name;
        Dialogue = dialogue;
        OfferedQuest = offeredQuest;
    }

    public string Talk() => Dialogue;

    public void AssignQuest(Quest quest)
    {
        ArgumentNullException.ThrowIfNull(quest);
        OfferedQuest = quest;
    }

    public Quest? RemoveQuest()
    {
        Quest? removedQuest = OfferedQuest;
        OfferedQuest = null;
        return removedQuest;
    }
}
