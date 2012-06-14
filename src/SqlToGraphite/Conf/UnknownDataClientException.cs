using System;

namespace SqlToGraphite
{
    public class UnknownDataClientException : Exception
    {
        public UnknownDataClientException(string message)
            : base(message)
        {
        }
    }
}