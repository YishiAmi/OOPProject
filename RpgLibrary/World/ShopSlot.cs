namespace RpgLibrary.World
{
    public class ShopSlot
    {
        public IShopItem Item { get; }
        public int Quantity { get; set; }

        public ShopSlot(IShopItem item, int quantity)
        {
            Item = item;
            Quantity = quantity;
        }
    }
}

