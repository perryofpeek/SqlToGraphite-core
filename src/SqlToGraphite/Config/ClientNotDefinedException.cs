using System;

namespace SqlToGraphite.Config
{
    public class ClientNotDefinedException : Exception
    {
        public ClientNotDefinedException(string message)
            : base(message)
        {
        }
    }
}