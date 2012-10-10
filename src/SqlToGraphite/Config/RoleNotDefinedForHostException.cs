using System;

namespace SqlToGraphite.Config
{
    public class RoleNotDefinedForHostException : Exception
    {
        public RoleNotDefinedForHostException(string message) : base(message)
        {
        }
    }
}