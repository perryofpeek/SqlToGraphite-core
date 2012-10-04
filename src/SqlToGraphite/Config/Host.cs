using System.Collections.Generic;
using System.Xml.Serialization;

namespace ConfigSpike.Config
{
    public class Host
    {
        public Host()
        {
            this.Roles = new List<Role>();
        }

        [XmlAttribute]
        public string Name { get; set; }

        public List<Role> Roles { get; set; }
    }
}