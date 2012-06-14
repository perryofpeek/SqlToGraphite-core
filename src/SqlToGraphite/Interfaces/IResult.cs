using System;

namespace SqlToGraphite
{
    public interface IResult
    {
        int Value { get; }

        DateTime TimeStamp { get; }

        string Name { get; }

        string Path { get; }

        string FullPath { get; }
    }
}