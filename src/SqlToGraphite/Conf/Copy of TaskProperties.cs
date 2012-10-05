namespace SqlToGraphite.Conf
{
    public class TaskProperties
    {
        public TaskProperties(string role, string frequency, string client, string connectionstring, string name, string path, string port, string sql, string type)
        {
            this.Role = role;
            this.Frequency = frequency;
            this.Client = client;
            this.Connectionstring = connectionstring;
            this.Name = name;
            this.Path = path;
            this.Port = port;
            this.Sql = sql;
            this.Type = type;
        }

        public string Role { get; set; }
        
        public string Frequency { get; set; }
        
        public string Client { get; set; }
        
        public string Connectionstring { get; set; }
        
        public string Name { get; set; }
        
        public string Path { get; set; }
        
        public string Port { get; set; }
        
        public string Sql { get; set; }
        
        public string Type { get; set; }
    }
}