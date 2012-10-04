using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace ConfigSpike.Config
{
    public class SqlToGraphiteConfig : IXmlSerializable
    {
        private static readonly Type[] JobTypes;

        private static readonly Type[] ClientTypes;

        static SqlToGraphiteConfig()
        {
            JobTypes = GetJobTypes().ToArray();
            ClientTypes = GetClientTypes().ToArray();
        }

        public SqlToGraphiteConfig()
        {
            this.Jobs = new List<Job>();
            this.Clients = new ListOfUniqueType<Client>();
            this.Hosts = new List<Host>();
            this.Templates = new List<Template>();
        }

        public ListOfUniqueType<Client> Clients { get; set; }

        public List<Job> Jobs { get; set; }

        public List<Host> Hosts { get; set; }

        public List<Template> Templates { get; set; }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void Validate()
        {
            // Check that the Jobs all have matching clients
            this.ValidateJobs();

            this.ValidateHosts();

            this.ValidateTemplates();
        }

        private void ValidateTemplates()
        {
            foreach (var template in this.Templates)
            {
                this.CheckAllTaskSetsHaveUseAJobWithName(template);
            }
        }

        private void CheckAllTaskSetsHaveUseAJobWithName(Template template)
        {
            foreach (var wi in template.WorkItems)
            {
                foreach (var taskset in wi.TaskSet)
                {
                    foreach (var task in taskset.Tasks.Where(task => !this.CheckJobNameExists(task)))
                    {
                        throw new JobNotDefinedForTaskException(string.Format("The job named {0} has not been defined for the task in role {1}", task.JobName, wi.RoleName));
                    }
                }
            }
        }

        private bool CheckJobNameExists(Task task)
        {
            bool rtn = false;
            foreach (var job in this.Jobs)
            {
                if (job.Name == task.JobName)
                {
                    rtn = true;
                }
            }

            return rtn;
        }

        private void ValidateHosts()
        {
            foreach (var host in this.Hosts)
            {
                this.CheckAllRolesExist(host);
            }
        }

        private void CheckAllRolesExist(Host host)
        {
            foreach (var role in host.Roles)
            {
                var rtn = false;
                foreach (var template in this.Templates)
                {
                    foreach (var wi in template.WorkItems)
                    {
                        if (role.Name == wi.RoleName)
                        {
                            rtn = true;
                        }
                    }
                }

                if (!rtn)
                {
                    throw new RoleNotDefinedForHostException(string.Format("The role {0} has not been defined for host {1}", role.Name, host.Name));
                }
            }
        }

        private void ValidateJobs()
        {
            foreach (var job in this.Jobs)
            {
                if (!this.ClientExist(job.ClientName))
                {
                    throw new ClientNotDefinedException(string.Format("The client {0} has not been defined", job.ClientName));
                }
            }
        }

        private bool ClientExist(string name)
        {
            var rtn = false;
            foreach (var c in this.Clients)
            {
                if (c.ClientName == name)
                {
                    rtn = true;
                }
            }

            return rtn;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            var wasEmpty = reader.IsEmptyElement;
            reader.Read();
            if (wasEmpty)
            {
                return;
            }

            reader.MoveToContent();
            reader.ReadStartElement("Jobs");
            this.Jobs = GenericSerializer.Deserialize<List<Job>>(reader, JobTypes);
            reader.ReadEndElement();

            reader.ReadStartElement("Clients");
            this.Clients = GenericSerializer.Deserialize<ListOfUniqueType<Client>>(reader, ClientTypes);
            reader.ReadEndElement();

            reader.ReadStartElement("Hosts");
            this.Hosts = GenericSerializer.Deserialize<List<Host>>(reader);
            reader.ReadEndElement();

            reader.ReadStartElement("Templates");
            this.Templates = GenericSerializer.Deserialize<List<Template>>(reader);
            reader.ReadEndElement();

            //Read Closing Element
            reader.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("Jobs");
            GenericSerializer.Serialize<List<Job>>(this.Jobs, writer, JobTypes);
            writer.WriteEndElement();

            writer.WriteStartElement("Clients");
            GenericSerializer.Serialize<ListOfUniqueType<Client>>(this.Clients, writer, ClientTypes);
            writer.WriteEndElement();

            writer.WriteStartElement("Hosts");
            GenericSerializer.Serialize<List<Host>>(this.Hosts, writer);
            writer.WriteEndElement();

            writer.WriteStartElement("Templates");
            GenericSerializer.Serialize<List<Template>>(this.Templates, writer);
            writer.WriteEndElement();
        }

        public static List<Type> GetClientTypes()
        {
            var types = new List<Type>();
            var asm = typeof(SqlToGraphiteConfig).Assembly;
            var client = typeof(Client);

            //Query our types. We could also load any other assemblies and
            //query them for any types that inherit from Animal
            foreach (Type currType in asm.GetTypes())
            {
                if (!currType.IsAbstract && !currType.IsInterface && client.IsAssignableFrom(currType))
                {
                    types.Add(currType);
                }
            }

            return types;
        }

        public static List<Type> GetJobTypes()
        {
            var types = new List<Type>();
            var asm = typeof(SqlToGraphiteConfig).Assembly;
            var job = typeof(Job);

            //Query our types. We could also load any other assemblies and
            //query them for any types that inherit from Animal
            foreach (Type currType in asm.GetTypes())
            {
                if (!currType.IsAbstract && !currType.IsInterface && job.IsAssignableFrom(currType))
                {
                    types.Add(currType);
                }
            }

            return types;
        }
    }
}