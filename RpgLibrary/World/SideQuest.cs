namespace RpgLibrary.World;

public sealed class SideQuest : Quest
{
    public string Region { get; }
    public override QuestType Type => QuestType.Side;

    public SideQuest(
        string title,
        string objective,
        int goldReward,
        string region,
        int requiredProgress = 1)
        : base(title, objective, goldReward, requiredProgress)
    {
        if (string.IsNullOrWhiteSpace(region))
        {
            throw new ArgumentException(
                "A side quest region is required.",
                nameof(region));
        }

        Region = region;
    }
}
