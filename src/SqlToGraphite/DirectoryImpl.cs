using System.Collections.Generic;
using System.IO;

namespace SqlToGraphite
{
    public class DirectoryImpl : IDirectory
    {
        public IList<string> GetFilesInCurrentDirectory(string filesToScan)
        {
            return Directory.GetFiles(Directory.GetCurrentDirectory(), filesToScan);
        }
    }
}