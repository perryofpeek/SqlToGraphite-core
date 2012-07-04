using SqlToGraphite;

namespace SqlToGraphiteInterfaces
{
    public class TaskParams : ITaskParams
    {
        public TaskParams(string path, string sql, string connectionString, string type, string name, string client)
        {
            this.Path = path;
            this.Sql = sql;
            this.ConnectionString = connectionString;
            this.Type = type;
            this.Name = name;
            this.Client = client;
        }      

        public string Path { get; private set;  }

        public string Sql { get; private set; }

        public string ConnectionString { get; private set; }

        public string Type { get; private set; }

        public string Name { get; private set; }

        public string Client { get; private set; }
    }
}