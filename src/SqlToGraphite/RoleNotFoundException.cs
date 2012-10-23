using System;

namespace SqlToGraphite.UnitTests
{
    public class RoleNotFoundException : Exception
    {
        public RoleNotFoundException(string message)
            : base(message)
        {
        }
    }
}