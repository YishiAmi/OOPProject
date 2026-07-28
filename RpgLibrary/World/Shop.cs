using System.Collections.Generic;
using RpgLibrary.Contracts;

namespace RpgLibrary.World;

public class Shop
{
    public string ShopName { get; }
    public List<ShopSlot> Inventory { get; } = new();

    public Shop(string shopName)
    {
        ShopName = shopName;
    }

    public bool Buy(IShopItem item, ref int playerGold)
    {
        if (playerGold >= item.Price)
        {
            playerGold -= item.Price;
            return true;
        }
        return false;
    }
}

/*
    Notes:

    -   since harith and ammar are working on their own pace I had them follow an interface so
        I can carry on with my code without relying on them, and we still getting brownie points
        for using interfaces >:).

    -   having a list<> on get; only still allows you to interact with it, just not set a new list
        which inturn deletes the whole shop.

    -   inventory is something to pause and think of. my initial plan is just make it so we build
        of the nice stuff of list and literally add items x times where x is our inventory count.

    -   update: where is the fun in the previous I'll just use an industry standart simple slot system.
        that would shift all the other namespaces dependancies into a lesser class, sweet!

*/