using System.Collections.Generic;
using System.Xml.Serialization;

namespace SqlToGraphite.Config
{
    public class WorkItems
    {
        public WorkItems()
        {
            this.TaskSet = new List<TaskSet>();
        }

        [XmlAttribute]
        public string RoleName { get; set; }

        public List<TaskSet> TaskSet { get; set; }
    }
}