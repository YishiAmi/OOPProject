using System;

namespace RpgLibrary.Exceptions
{
    public class InventoryFullException : Exception
    {
        public InventoryFullException(): base("Inventory is full.")
        {
            
        }

        public InventoryFullException(string message): base(message)
        {
            
        }

        public InventoryFullException(string message, Exception inner): base(message, inner)
        {
            
        }
    }
}