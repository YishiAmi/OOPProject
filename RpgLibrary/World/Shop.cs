using System.Collections.ObjectModel;
using RpgLibrary.Contracts;

namespace RpgLibrary.World;

public sealed class Shop
{
    private readonly List<ShopSlot> _inventory = new();
    private readonly ReadOnlyCollection<ShopSlot> _inventoryView;

    public string ShopName { get; }
    public IReadOnlyList<ShopSlot> Inventory => _inventoryView;

    public Shop(string shopName)
    {
        if (string.IsNullOrWhiteSpace(shopName))
        {
            throw new ArgumentException("A shop name is required.", nameof(shopName));
        }

        ShopName = shopName;
        _inventoryView = _inventory.AsReadOnly();
    }

    public ShopSlot AddStock(IShopItem item, int quantity)
    {
        ArgumentNullException.ThrowIfNull(item);

        if (item.Price < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(item),
                "An item's price cannot be negative.");
        }

        if (quantity < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantity),
                "Stock quantity must be at least one.");
        }

        ShopSlot? existingSlot = _inventory.Find(
            slot => ReferenceEquals(slot.Item, item));

        if (existingSlot is not null)
        {
            existingSlot.Restock(quantity);
            return existingSlot;
        }

        ShopSlot newSlot = new(item, quantity);
        _inventory.Add(newSlot);
        return newSlot;
    }

    public bool RemoveStock(IShopItem item, int quantity)
    {
        ArgumentNullException.ThrowIfNull(item);

        if (quantity < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantity),
                "The quantity to remove must be at least one.");
        }

        ShopSlot? slot = _inventory.Find(
            candidate => ReferenceEquals(candidate.Item, item));

        return slot is not null && slot.TryRemove(quantity);
    }

    internal PurchaseStatus TryTake(
        ShopSlot slot,
        int availableGold,
        out int purchasePrice)
    {
        ArgumentNullException.ThrowIfNull(slot);
        purchasePrice = 0;

        if (availableGold < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(availableGold),
                "Available gold cannot be negative.");
        }

        if (!_inventory.Contains(slot))
        {
            return PurchaseStatus.ItemNotSold;
        }

        int currentPrice = slot.Item.Price;

        if (currentPrice < 0)
        {
            throw new InvalidOperationException(
                "A stocked item's price cannot be negative.");
        }

        if (!slot.IsInStock)
        {
            return PurchaseStatus.OutOfStock;
        }

        if (availableGold < currentPrice)
        {
            return PurchaseStatus.InsufficientGold;
        }

        if (!slot.TryTakeOne())
        {
            return PurchaseStatus.OutOfStock;
        }

        purchasePrice = currentPrice;
        return PurchaseStatus.Success;
    }
}
