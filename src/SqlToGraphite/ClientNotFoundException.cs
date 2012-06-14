using System;

namespace SqlToGraphite.UnitTests
{
    public class ClientNotFoundException : Exception
    {
        public ClientNotFoundException(string message)
            : base(message)
        {
        }
    }
}