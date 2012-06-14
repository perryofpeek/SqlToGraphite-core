using System;

namespace SqlToGraphite
{
    public class UnknownGraphiteClientTypeException : Exception
    {
        public UnknownGraphiteClientTypeException(string message) : base(message)
        {
        }
    }
}