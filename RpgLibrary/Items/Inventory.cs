using System.Collections.Generic;


namespace RPGGameLibrary.Items
{
    public class Inventory
    {

        public List<Item> Items { get; set; }


        public int Capacity { get; set; }


        public Inventory(int capacity)
        {
            Capacity = capacity;

            Items = new List<Item>();
        }



        public bool AddItem(Item item)
        {

            if(Items.Count < Capacity)
            {
                Items.Add(item);

                Console.WriteLine(
                    $"{item.Name} added"
                );

                return true;
            }


            Console.WriteLine(
                "Inventory Full!"
            );


            return false;
        }




        public void RemoveItem(Item item)
        {
            if(Items.Contains(item))
            {
                Items.Remove(item);

                Console.WriteLine(
                    $"{item.Name} removed"
                );
            }
        }





        public void ShowInventory()
        {
            Console.WriteLine(
                "===== INVENTORY ====="
            );


            foreach(Item item in Items)
            {
                Console.WriteLine(
                    $"{item.Name}"
                );
            }
        }



        public int GetItemCount()
        {
            return Items.Count;
        }

    }
}
