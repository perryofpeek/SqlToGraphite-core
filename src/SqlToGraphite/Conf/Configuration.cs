using System.Collections.Generic;

namespace SqlToGraphite
{
    public class HostConfiguration 
    {
        public HostConfiguration()
        {
            Tasks = new List<Task>();
        }

        public IList<Task> Tasks { get; set; }

        public string Hostname { get; set; }

        public int Port { get; set; }

        public int Frequency { get; set; }
    }
}