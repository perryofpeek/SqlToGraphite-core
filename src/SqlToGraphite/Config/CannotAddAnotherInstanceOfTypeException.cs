using System;

namespace ConfigSpike.Config
{
    public class CannotAddAnotherInstanceOfTypeException : Exception
    {
        public CannotAddAnotherInstanceOfTypeException(string message) : base(message)
        {
        }
    }
}