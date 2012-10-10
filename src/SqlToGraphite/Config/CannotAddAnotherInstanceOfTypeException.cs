using System;

namespace SqlToGraphite.Config
{
    public class CannotAddAnotherInstanceOfTypeException : Exception
    {
        public CannotAddAnotherInstanceOfTypeException(string message) : base(message)
        {
        }
    }
}