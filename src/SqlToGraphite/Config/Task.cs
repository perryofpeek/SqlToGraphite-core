using System.Xml.Serialization;

namespace SqlToGraphite.Config
{
    public class Task
    {
        [XmlAttribute]
        public string JobName { get; set; }
    }
}