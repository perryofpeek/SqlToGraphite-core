using System.Xml.Serialization;

namespace ConfigSpike.Config
{
    public class Role
    {
        [XmlAttribute]
        public string Name { get; set; }
    }
}