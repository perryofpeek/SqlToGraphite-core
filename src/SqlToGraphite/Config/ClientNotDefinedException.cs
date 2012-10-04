using System;

namespace ConfigSpike.Config
{
    public class ClientNotDefinedException : Exception
    {
        public ClientNotDefinedException(string message)
            : base(message)
        {
        }
    }
}