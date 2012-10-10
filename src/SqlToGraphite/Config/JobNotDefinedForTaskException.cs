using System;

namespace SqlToGraphite.Config
{
    public class JobNotDefinedForTaskException : Exception
    {
        public JobNotDefinedForTaskException(string message) : base(message)
        {
        }
    }
}