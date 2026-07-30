namespace RpgLibrary.World;

public enum QuestActionStatus
{
    Success,
    NpcNotInCurrentMap,
    NoQuestOffered,
    AlreadyAccepted,
    QuestNotActive,
    InvalidProgress,
    RequirementsNotMet,
    AlreadyCompleted
}
