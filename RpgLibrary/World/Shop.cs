using RpgLibrary.Contracts;

namespace RpgLibrary.World;

public class Shop
{
    public string Name { get; set; } = string.Empty;

    public IList<IShopItem> Items { get; } = new List<IShopItem>();
}
