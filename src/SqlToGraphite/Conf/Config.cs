using System.Collections.Generic;
using System.IO;

using SqlToGraphite.Clients;
using SqlToGraphite.Conf;

namespace SqlToGraphite
{
    //public class Config : IConfig
    //{
    //    private readonly Dictionary<string, SqlToGraphiteConfigClientsClient> clientList;

    //    private readonly List<SqlToGraphiteConfigTemplatesWorkItemsTaskSet> taskSets;

    //    public Config(TextReader sr, KnownGraphiteClients knownGraphiteClients)
    //    {
    //        clientList = new Dictionary<string, SqlToGraphiteConfigClientsClient>();
    //        taskSets = new List<SqlToGraphiteConfigTemplatesWorkItemsTaskSet>();
    //        var ser = new System.Xml.Serialization.XmlSerializer(typeof(SqlToGraphiteConfig));
    //        var o = ser.Deserialize(sr);
    //        var c = (SqlToGraphiteConfig)o;
    //        foreach (var item in c.Items)
    //        {
    //            if (item.GetType() == typeof(SqlToGraphiteConfigClients))
    //            {
    //                this.AddAllClients(knownGraphiteClients, item);
    //            }
    //            else if (item.GetType() == typeof(SqlToGraphiteConfigTemplates))
    //            {
    //                var configTemplates = (SqlToGraphiteConfigTemplates)item;
    //            }
    //            else
    //            {
    //                var roles = (SqlToGraphiteConfigHosts)item;
    //            }
    //        }
    //    }

    //    //private void AddAllWorkItems(object item)
    //    // {
    //    //    Templates templates = new Templates()
    //    //    var i = (SqlToGraphiteConfigTemplatesWorkItemsTaskSetTask)item;
    //    //this.Hostname = i.hostname;
    //    //foreach (var configWorkItem in i.TaskSet)
    //    //{
    //    //    this.taskSets.Add(configWorkItem);
    //    //}
    //    // }
    //    private void AddAllClients(KnownGraphiteClients knownGraphiteClients, object item)
    //    {
    //        var i = (SqlToGraphiteConfigClients)item;
    //        foreach (var client in i.Client)
    //        {
    //            this.AddKnownClient(knownGraphiteClients, client);
    //        }
    //    }

    //    private void AddKnownClient(KnownGraphiteClients knownGraphiteClients, SqlToGraphiteConfigClientsClient client)
    //    {
    //        if (knownGraphiteClients.IsKnown(client.name))
    //        {
    //            this.clientList.Add(client.name, client);
    //        }
    //        else
    //        {
    //            throw new UnknownGraphiteClientTypeException(string.Format("{0} is unknown", client.name));
    //        }
    //    }

    //    public string Hostname { get; private set; }

    //    public Dictionary<string, SqlToGraphiteConfigClientsClient> GetClientList()
    //    {
    //        return this.clientList;
    //    }

    //    public List<SqlToGraphiteConfigTemplatesWorkItemsTaskSet> GetWorkItems()
    //    {
    //        return this.taskSets;
    //    }
    //}
}