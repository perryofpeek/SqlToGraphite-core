namespace SqlToGraphite
{
    public class GraphiteParams1
    {
        public int Port { get; private set; }

        public string Hostname { get; private set; }

        public GraphiteParams1(string hostname, int port)
        {
            this.Port = port;
            this.Hostname = hostname;            
        }
    }
}