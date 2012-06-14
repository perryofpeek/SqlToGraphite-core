namespace SqlToGraphite
{
    public class GraphiteParams
    {
        public int Port { get; private set; }

        public string Hostname { get; private set; }

        public GraphiteParams(string hostname, int port)
        {
            this.Port = port;
            this.Hostname = hostname;            
        }
    }
}