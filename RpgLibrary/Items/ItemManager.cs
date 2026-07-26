using System.Collections.Generic;


namespace RPGGameLibrary.Items
{
    public class ItemManager
    {
        public List<Item> AvailableItems { get; set; }


        public ItemManager()
        {
            AvailableItems = new List<Item>();
        }



        public void AddItem(Item item)
        {
            AvailableItems.Add(item);

            Console.WriteLine(
                $"{item.Name} added to item database"
            );
        }



        public void RemoveItem(Item item)
        {
            AvailableItems.Remove(item);

            Console.WriteLine(
                $"{item.Name} removed"
            );
        }



        public void DisplayItems()
        {
            Console.WriteLine(
                "=== Available Items ==="
            );


            foreach(Item item in AvailableItems)
            {
                Console.WriteLine(
                    $"{item.Name} - {item.Description}"
                );
            }
        }
    }
}
