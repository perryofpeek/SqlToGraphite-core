namespace Graphite
{
    using System;

    public interface IGraphiteMetric
    {
        string Path { get; }

        string Value { get; }

        long UnixTimeStamp { get; }

        DateTime DotNetTimeStamp { get; }

        string ToGraphiteMessage();
    }
}