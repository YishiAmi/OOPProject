using System;
namespace RpgLibrary.Exceptions
{
    public class NotEnoughGoldException : Exception
    {
        
        public NotEnoughGoldException() : base ("Not Enough Gold!")
        {
            
        }

        public NotEnoughGoldException(string message) : base (message)
        {
            
        }

        public NotEnoughGoldException(string message, Exception inner) : base (message, inner)
        {
            
        }
    }
}