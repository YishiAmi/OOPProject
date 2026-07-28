namespace RpgLibrary.World;

public class NPC
{
    public string Name { get; }
    public string Dialogue { get; }
    public Quest? OfferedQuest { get; set; }

    public NPC(string name, string dialogue)
    {
        Name = name;
        Dialogue = dialogue;
    }

    public string Talk()
    {
        return Dialogue;
    }
}


/*
    Notes:

    -   think bro not all npcs have quests (?) is used to indicate the property can be null/blank.

*/