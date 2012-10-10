using System.Xml.Serialization;

namespace SqlToGraphite.Config
{
    public class Role
    {
        [XmlAttribute]
        public string Name { get; set; }
    }
}