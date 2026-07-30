using RpgLibrary.Contracts;

namespace RpgLibrary.World;

public sealed class ShopSlot
{
    public IShopItem Item { get; }
    public int Quantity { get; private set; }
    public bool IsInStock => Quantity > 0;

    public ShopSlot(IShopItem item, int quantity)
    {
        ArgumentNullException.ThrowIfNull(item);

        if (item.Price < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(item),
                "An item's price cannot be negative.");
        }

        if (quantity < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantity),
                "A shop slot quantity cannot be negative.");
        }

        Item = item;
        Quantity = quantity;
    }

    internal bool TryTakeOne()
    {
        if (!IsInStock)
        {
            return false;
        }

        Quantity--;
        return true;
    }

    internal void Restock(int quantity)
    {
        if (quantity < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantity),
                "Restock quantity must be at least one.");
        }

        Quantity = checked(Quantity + quantity);
    }

    internal bool TryRemove(int quantity)
    {
        if (quantity < 1 || quantity > Quantity)
        {
            return false;
        }

        Quantity -= quantity;
        return true;
    }
}
