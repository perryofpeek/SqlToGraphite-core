using System;

namespace SqlToGraphite.UnitTests
{
    public class HostNotFoundException : Exception
    {
        public HostNotFoundException(string message) : base(message)
        {
        }
    }
}