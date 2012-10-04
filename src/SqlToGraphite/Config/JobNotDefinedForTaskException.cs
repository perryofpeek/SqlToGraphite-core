using System;

namespace ConfigSpike.Config
{
    public class JobNotDefinedForTaskException : Exception
    {
        public JobNotDefinedForTaskException(string message) : base(message)
        {
        }
    }
}