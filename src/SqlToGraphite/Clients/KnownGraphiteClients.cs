using System.Collections.Generic;

namespace SqlToGraphite.Clients
{
    public class KnownGraphiteClients1 : IKnownGraphiteClients
    {
        private List<string> list; 

        public KnownGraphiteClients1()
        {
            list = new List<string> { "statsdudp", "graphitetcp", "graphiteudp" };
        }

        public bool IsKnown(string client)
        {
            return list.Contains(client.ToLower());
        } 
    }
}