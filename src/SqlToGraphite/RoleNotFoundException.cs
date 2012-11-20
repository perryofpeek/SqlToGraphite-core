namespace SqlToGraphite
{
    using System;

    public class RoleNotFoundException : Exception
    {
        public RoleNotFoundException(string message)
            : base(message)
        {
        }
    }
}