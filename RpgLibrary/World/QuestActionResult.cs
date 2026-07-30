namespace RpgLibrary.World;

public sealed record QuestActionResult(
    QuestActionStatus Status,
    Quest? Quest,
    int GoldAwarded,
    int GoldRemaining)
{
    public bool Succeeded => Status == QuestActionStatus.Success;
}
