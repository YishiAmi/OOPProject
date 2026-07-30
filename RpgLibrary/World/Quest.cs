namespace RpgLibrary.World;

public abstract class Quest
{
    public string Title { get; }
    public string Objective { get; }
    public int GoldReward { get; }
    public int RequiredProgress { get; }
    public int CurrentProgress { get; private set; }
    public QuestStatus Status { get; private set; }
    public abstract QuestType Type { get; }

    public bool Completed => Status == QuestStatus.Completed;

    public bool CanComplete =>
        Status == QuestStatus.Active &&
        CurrentProgress >= RequiredProgress;

    protected Quest(
        string title,
        string objective,
        int goldReward,
        int requiredProgress = 1)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("A quest title is required.", nameof(title));
        }

        if (string.IsNullOrWhiteSpace(objective))
        {
            throw new ArgumentException("A quest objective is required.", nameof(objective));
        }

        if (goldReward < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(goldReward),
                "A quest reward cannot be negative.");
        }

        if (requiredProgress < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(requiredProgress),
                "Required progress must be at least one.");
        }

        Title = title;
        Objective = objective;
        GoldReward = goldReward;
        RequiredProgress = requiredProgress;
        Status = QuestStatus.Available;
    }

    internal bool Accept()
    {
        if (Status != QuestStatus.Available)
        {
            return false;
        }

        Status = QuestStatus.Active;
        return true;
    }

    internal bool AddProgress(int amount)
    {
        if (Status != QuestStatus.Active || amount <= 0)
        {
            return false;
        }

        long updatedProgress = (long)CurrentProgress + amount;
        CurrentProgress = (int)Math.Min(RequiredProgress, updatedProgress);

        return true;
    }

    internal bool Complete()
    {
        if (!CanComplete)
        {
            return false;
        }

        Status = QuestStatus.Completed;
        return true;
    }
}
