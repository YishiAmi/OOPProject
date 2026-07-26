using RpgLibrary.Contracts;

namespace RpgLibrary.Items;

public class Item : IShopItem
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int Price { get; set; }
}
