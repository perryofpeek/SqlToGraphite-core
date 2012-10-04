using System.Xml.Serialization;

namespace ConfigSpike.Config
{
    public class Task
    {
        [XmlAttribute]
        public string JobName { get; set; }
    }
}