namespace RpgLibrary.World;

public sealed record NpcInteractionResult(
    NpcInteractionStatus Status,
    NPC Npc,
    string Dialogue,
    Quest? OfferedQuest,
    bool CanAcceptQuest)
{
    public bool Succeeded => Status == NpcInteractionStatus.Success;
}
