using System.Collections.Generic;

namespace SqlToGraphite
{
    public interface IDirectory
    {
        IList<string> GetFilesInCurrentDirectory(string filesToScan);
    }
}