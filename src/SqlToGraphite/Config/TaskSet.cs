using System.Collections.Generic;
using System.Xml.Serialization;

namespace ConfigSpike.Config
{
    public class TaskSet
    {
        public TaskSet()
        {
            this.Tasks = new List<Task>();
        }

        [XmlAttribute]
        public int Frequency { get; set; }

        public List<Task> Tasks { get; set; }
    }
}