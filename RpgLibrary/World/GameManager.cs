using System.Collections.ObjectModel;
using RpgLibrary.Contracts;

namespace RpgLibrary.World;

public sealed class GameManager
{
    private readonly List<Quest> _activeQuests = new();
    private readonly List<Quest> _completedQuests = new();
    private readonly ReadOnlyCollection<Quest> _activeQuestView;
    private readonly ReadOnlyCollection<Quest> _completedQuestView;

    public ICombatant Player { get; }
    public Map CurrentMap { get; private set; }
    public int PlayerGold { get; private set; }
    public IReadOnlyList<Quest> ActiveQuests => _activeQuestView;
    public IReadOnlyList<Quest> CompletedQuests => _completedQuestView;

    public GameManager(
        ICombatant player,
        Map startingMap,
        int startingGold = 0)
    {
        ArgumentNullException.ThrowIfNull(player);
        ArgumentNullException.ThrowIfNull(startingMap);

        if (startingGold < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(startingGold),
                "Starting gold cannot be negative.");
        }

        Player = player;
        CurrentMap = startingMap;
        PlayerGold = startingGold;
        _activeQuestView = _activeQuests.AsReadOnly();
        _completedQuestView = _completedQuests.AsReadOnly();
    }

    public Map ChangeMap(Map newMap)
    {
        ArgumentNullException.ThrowIfNull(newMap);
        CurrentMap = newMap;
        return CurrentMap;
    }

    public NpcInteractionResult TalkToNpc(NPC npc)
    {
        ArgumentNullException.ThrowIfNull(npc);

        if (!CurrentMap.ContainsNpc(npc))
        {
            return new NpcInteractionResult(
                NpcInteractionStatus.NpcNotInCurrentMap,
                npc,
                string.Empty,
                null,
                false);
        }

        Quest? offeredQuest = npc.OfferedQuest;

        return new NpcInteractionResult(
            NpcInteractionStatus.Success,
            npc,
            npc.Talk(),
            offeredQuest,
            offeredQuest?.Status == QuestStatus.Available);
    }

    public QuestActionResult AcceptQuest(NPC npc)
    {
        ArgumentNullException.ThrowIfNull(npc);

        if (!CurrentMap.ContainsNpc(npc))
        {
            return QuestResult(
                QuestActionStatus.NpcNotInCurrentMap,
                null);
        }

        Quest? quest = npc.OfferedQuest;

        if (quest is null)
        {
            return QuestResult(QuestActionStatus.NoQuestOffered);
        }

        if (quest.Status == QuestStatus.Completed)
        {
            return QuestResult(QuestActionStatus.AlreadyCompleted, quest);
        }

        if (quest.Status == QuestStatus.Active)
        {
            return QuestResult(QuestActionStatus.AlreadyAccepted, quest);
        }

        if (!quest.Accept())
        {
            return QuestResult(QuestActionStatus.AlreadyAccepted, quest);
        }

        _activeQuests.Add(quest);
        return QuestResult(QuestActionStatus.Success, quest);
    }

    public QuestActionResult AddQuestProgress(Quest quest, int amount)
    {
        ArgumentNullException.ThrowIfNull(quest);

        if (amount <= 0)
        {
            return QuestResult(QuestActionStatus.InvalidProgress, quest);
        }

        if (quest.Status == QuestStatus.Completed)
        {
            return QuestResult(QuestActionStatus.AlreadyCompleted, quest);
        }

        if (!_activeQuests.Contains(quest) ||
            quest.Status != QuestStatus.Active)
        {
            return QuestResult(QuestActionStatus.QuestNotActive, quest);
        }

        return quest.AddProgress(amount)
            ? QuestResult(QuestActionStatus.Success, quest)
            : QuestResult(QuestActionStatus.InvalidProgress, quest);
    }

    public QuestActionResult CompleteQuest(Quest quest)
    {
        ArgumentNullException.ThrowIfNull(quest);

        if (quest.Status == QuestStatus.Completed)
        {
            return QuestResult(QuestActionStatus.AlreadyCompleted, quest);
        }

        if (!_activeQuests.Contains(quest) ||
            quest.Status != QuestStatus.Active)
        {
            return QuestResult(QuestActionStatus.QuestNotActive, quest);
        }

        if (!quest.CanComplete)
        {
            return QuestResult(QuestActionStatus.RequirementsNotMet, quest);
        }

        int updatedGold = checked(PlayerGold + quest.GoldReward);

        if (!quest.Complete())
        {
            return QuestResult(QuestActionStatus.RequirementsNotMet, quest);
        }

        _activeQuests.Remove(quest);
        _completedQuests.Add(quest);
        PlayerGold = updatedGold;

        return QuestResult(
            QuestActionStatus.Success,
            quest,
            quest.GoldReward);
    }

    public Shop? GetCurrentShop() => CurrentMap.LocalShop;

    public PurchaseResult BuyItem(ShopSlot slot)
    {
        ArgumentNullException.ThrowIfNull(slot);

        Shop? shop = GetCurrentShop();

        if (shop is null)
        {
            return new PurchaseResult(
                PurchaseStatus.ShopUnavailable,
                slot.Item,
                0,
                PlayerGold);
        }

        PurchaseStatus status = shop.TryTake(
            slot,
            PlayerGold,
            out int purchasePrice);

        if (status == PurchaseStatus.Success)
        {
            PlayerGold -= purchasePrice;
        }

        return new PurchaseResult(
            status,
            slot.Item,
            purchasePrice,
            PlayerGold);
    }

    public int AwardGold(int amount)
    {
        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                "Awarded gold cannot be negative.");
        }

        PlayerGold = checked(PlayerGold + amount);
        return PlayerGold;
    }

    private QuestActionResult QuestResult(
        QuestActionStatus status,
        Quest? quest = null,
        int goldAwarded = 0)
    {
        return new QuestActionResult(
            status,
            quest,
            goldAwarded,
            PlayerGold);
    }
}
