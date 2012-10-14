using System.IO;

using SqlToGraphite.Conf;

namespace SqlToGraphite
{
    public class ConfigFileWriter : IConfigWriter
    {
        private readonly string fileName;

        public ConfigFileWriter(string fileName)
        {
            this.fileName = fileName;
        }

        public void Save(string data)
        {
            File.WriteAllText(fileName, data);
        }

        public void Save(string data, string path)
        {
            File.WriteAllText(path, data);
        }
    }
}