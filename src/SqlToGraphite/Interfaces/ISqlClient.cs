using System.Collections.Generic;

namespace SqlToGraphite
{
    public interface ISqlClient
    {
        IList<IResult> Get();
    }
}