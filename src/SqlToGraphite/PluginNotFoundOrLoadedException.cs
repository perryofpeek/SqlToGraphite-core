using System;

namespace SqlToGraphite
{
    public class PluginNotFoundOrLoadedException : Exception
    {
        public PluginNotFoundOrLoadedException(string message)
            : base(message)
        {
        }
    }
}