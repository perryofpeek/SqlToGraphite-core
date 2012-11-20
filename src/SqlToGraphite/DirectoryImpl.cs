using System.Collections.Generic;
using System.IO;

namespace SqlToGraphite
{
    public class DirectoryImpl : IDirectory
    {
        public IList<string> GetFilesInCurrentDirectory(string filesToScan)
        {
            var location = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var dirName = Path.GetDirectoryName(location);
            return Directory.GetFiles(dirName, filesToScan);
        }
    }
}