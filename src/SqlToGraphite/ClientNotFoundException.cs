namespace SqlToGraphite
{
    using System;

    internal class ClientNotFoundException : Exception
    {
        public ClientNotFoundException(string message)
            : base(message)
        {
        }
    }
}