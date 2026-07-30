namespace RpgLibrary.World;

public sealed class MainQuest : Quest
{
    public int Chapter { get; }
    public override QuestType Type => QuestType.Main;

    public MainQuest(
        string title,
        string objective,
        int goldReward,
        int chapter,
        int requiredProgress = 1)
        : base(title, objective, goldReward, requiredProgress)
    {
        if (chapter < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(chapter),
                "A chapter number must be at least one.");
        }

        Chapter = chapter;
    }
}
