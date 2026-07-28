using System;
namespace RpgLibrary.Exceptions
{
    public class UltimateNotChargedException : Exception
    {
        public UltimateNotChargedException() : base("Ultimate is not ready to use!")
        {
            
        }

        public UltimateNotChargedException(string message) : base (message)
        {
            
        }

        public UltimateNotChargedException(string message, Exception inner) : base (message, inner)
        {
            
        }
    }
}