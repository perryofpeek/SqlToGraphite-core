using System;

using ConfigSpike.Config;

namespace SqlToGraphite
{
    public interface IAssemblyResolver
    {
        Type ResolveType(Job job);

        Type[] ResolveTypes(JobImpl job);
    }
}