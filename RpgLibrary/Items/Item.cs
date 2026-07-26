namespace RPGGameLibrary.Items
{
    public class Item
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int Value { get; set; }


        public Item(
            string name,
            string description,
            int value)
        {
            Name = name;
            Description = description;
            Value = value;
        }


        public virtual void Use()
        {
            Console.WriteLine($"Using {Name}");
        }


        public void DisplayInfo()
        {
            Console.WriteLine(
                $"Item: {Name}"
            );

            Console.WriteLine(
                $"Description: {Description}"
            );

            Console.WriteLine(
                $"Value: {Value}"
            );
        }
    }
}
