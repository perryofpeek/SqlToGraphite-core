using System.Collections.Generic;

using SqlToGraphite.UnitTests;

namespace SqlToGraphite
{
    public class GraphiteClients
    {
        private readonly Dictionary<string, GraphiteClient> clients;

        public GraphiteClients()
        {
            clients = new Dictionary<string, GraphiteClient>();
        }

        public void Add(string name, string port)
        {
            var n = name.ToLower();
            var p = int.Parse(port);
            clients.Add(n, new GraphiteClient(n, p));
        }

        public GraphiteClient Get(string name)
        {
            if (clients.ContainsKey(name.ToLower()))
            {
                return clients[name.ToLower()];
            }

            throw new ClientNotFoundException(string.Format("Client {0} is not known add this into the conifiguration xml", name));
        }
    }
}