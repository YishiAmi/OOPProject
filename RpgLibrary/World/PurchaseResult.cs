using RpgLibrary.Contracts;

namespace RpgLibrary.World;

public sealed record PurchaseResult(
    PurchaseStatus Status,
    IShopItem Item,
    int PricePaid,
    int GoldRemaining)
{
    public bool Succeeded => Status == PurchaseStatus.Success;
}
