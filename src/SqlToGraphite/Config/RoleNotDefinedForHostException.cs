using System;

namespace ConfigSpike.Config
{
    public class RoleNotDefinedForHostException : Exception
    {
        public RoleNotDefinedForHostException(string message) : base(message)
        {
        }
    }
}