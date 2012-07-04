using System;

namespace SqlToGraphiteInterfaces
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