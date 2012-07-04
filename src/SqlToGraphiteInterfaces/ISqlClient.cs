using System.Collections.Generic;

namespace SqlToGraphiteInterfaces
{
    public interface ISqlClient
    {
        IList<IResult> Get();
    }
}