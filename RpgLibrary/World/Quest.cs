namespace RpgLibrary.World;

public class Quest
{
    public string Title { get; set; } = string.Empty;

    public string Objective { get; set; } = string.Empty;

    public bool IsCompleted { get; set; }
}
