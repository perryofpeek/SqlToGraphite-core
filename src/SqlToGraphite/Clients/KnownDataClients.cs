using System.Collections.Generic;

namespace SqlToGraphite.Clients
{
    public class KnownDataClients : IKnownDataClients
    {
        private List<string> list;

        public KnownDataClients()
        {
            list = new List<string> { "sqlserver", "oracle", "wmi" };
        }

        public bool IsKnown(string client)
        {
            return list.Contains(client.ToLower());
        }
    }
}