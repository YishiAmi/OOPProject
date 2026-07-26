using RpgLibrary.Contracts;

namespace RpgLibrary.Items;

public class Inventory
{
    public int Gold { get; set; }

    public IList<IShopItem> Items { get; } = new List<IShopItem>();
}
