namespace SqlToGraphite
{
    using System;

    public class HostNotFoundException : Exception
    {
        public HostNotFoundException(string message) : base(message)
        {
        }
    }
}