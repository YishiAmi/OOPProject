namespace RpgLibrary.World;

public abstract class Quest
{
    public string Title { get; }  
    public string Objective { get; }
    public bool Completed { get; protected set; }
    public int GoldReward { get; }

    public Quest(string title, string objective, int goldReward)
    {
        Title = title;
        Objective = objective;
        GoldReward = goldReward;
        Completed = false;
    }

    public virtual void Complete()
    {
        Completed = true;
    }
}


/*
    Notes:

    -   readonly is for fields (raw variables). if you wrote public readonly string title, 
        you would be exposing a raw piece of data directly to the outside world essenially
        they are the same but hey a nice technical optimization is always appreciated.
    
    -   why not make quest abstract and have main and side quests to demonstrait inhertence,
        sweet more brownie points. oh and in that case make Completed protected rather than privately set.

*/