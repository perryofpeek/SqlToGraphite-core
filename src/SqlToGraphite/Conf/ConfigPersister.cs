using System.Collections.Generic;
using System.IO;

namespace SqlToGraphite.Conf
{
    public class ConfigPersister : IConfigPersister
    {
        private readonly IConfigWriter configWriter;

        public ConfigPersister(IConfigWriter configWriter)
        {
            this.configWriter = configWriter;
        }

        public void Save(List<SqlToGraphiteConfigClientsClient> clients, List<SqlToGraphiteConfigTemplatesWorkItems> templates, List<SqlToGraphiteConfigHostsHost> hosts)
        {
            var sqlToGraphiteConfig = new SqlToGraphiteConfig { Items = new object[3] };
            sqlToGraphiteConfig.Items[0] = new SqlToGraphiteConfigClients { Client = clients.ToArray() };
            sqlToGraphiteConfig.Items[1] = new SqlToGraphiteConfigTemplates { WorkItems = templates.ToArray() };
            sqlToGraphiteConfig.Items[2] = new SqlToGraphiteConfigHosts { host = hosts.ToArray() };

            var stringStream = new StringWriter();
            var ser = new System.Xml.Serialization.XmlSerializer(typeof(SqlToGraphiteConfig));
            ser.Serialize(stringStream, sqlToGraphiteConfig);
            configWriter.Save(stringStream.ToString());
        }
    }
}