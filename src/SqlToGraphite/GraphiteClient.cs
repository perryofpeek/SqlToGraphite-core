namespace SqlToGraphite
{
    public class GraphiteClient
    {
        public GraphiteClient(string name, int port)
        {
            Name = name;
            Port = port;
        }

        public string Name { get; private set; }

        public int Port { get; private set; }
    }
}