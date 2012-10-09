using System;

namespace SqlToGraphite
{
    public interface IAssemblyResolver
    {
        Type ResolveType(Job job);
    }
}