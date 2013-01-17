using System;

namespace Graphite
{
    using System.Collections.Generic;

    public interface IGraphiteClient
    {
        void Send(string path, string value, DateTime timeStamp);

        void Send(string path, int value, DateTime timeStamp);
        
        void Send(string path, long value, DateTime timeStamp);
        
        void Send(string path, decimal value, DateTime timeStamp);
        
        void Send(string path, float value, DateTime timeStamp);

        void Send(GraphiteMetrics graphiteMetrics);
    }
} 