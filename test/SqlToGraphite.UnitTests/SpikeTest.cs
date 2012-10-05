using System;
using System.Collections.Generic;
using System.Linq;

using ConfigSpike.Config;

using NUnit.Framework;

namespace ConfigSpike
{
    [TestFixture]
// ReSharper disable InconsistentNaming
    public class SpikeTest
    {
        [Test]
        public void Should()
        {
            try
            {
                var s1 = new SqlServer { ClientName = "s1Client", Name = "s1Name" };
                var s2 = new SqlServer { ClientName = "s2Client", Name = "s2Name" };
                var w1 = new WmiPlugin { ClientName = "w1Client", Hostname = "w1Hostname", Name = "SomeName", Sql = "w1Sql" };
                var c1 = new GraphiteTcpClient { ClientName = "c1", Port = 123 };
                var c2 = new GraphiteUdpClient { ClientName = "c2", Port = 1234 };

                var hh = new ConfigSpike.Config.SqlToGraphiteConfig();
                hh.Jobs.Add(s1);
                hh.Jobs.Add(s2);
                hh.Jobs.Add(w1);
                var r1 = new Role { Name = "role1" };
                var h1 = new Host { Name = "host1" };
                h1.Roles.Add(r1);
                hh.Hosts.Add(h1);

                var r2 = new Role { Name = "role2" };
                var r3 = new Role { Name = "role2" };
                var h2 = new Host { Name = "host2" };
                h1.Roles.Add(r2);
                h1.Roles.Add(r3);
                hh.Hosts.Add(h2);

                var template = new Template();
                var wi = new WorkItems();
                wi.RoleName = "role1";
                wi.TaskSet = new List<TaskSet>();
                var taskSet = new TaskSet();
                taskSet.Frequency = 123;
                var t1 = new Task { JobName = "t1Name" };
                var t2 = new Task { JobName = "t2Name" };
                taskSet.Tasks.Add(t1);
                taskSet.Tasks.Add(t2);

                wi.TaskSet.Add(taskSet);
                template.WorkItems.Add(wi);

                hh.Templates.Add(template);
                hh.Clients = new ListOfUniqueType<IClient> { c1, c2 };
                var genericSerializer = new GenericSerializer();
                string xml = genericSerializer.Serialize<ConfigSpike.Config.SqlToGraphiteConfig>(hh);
                Console.WriteLine(xml);
                var sqlToGraphiteConfig = genericSerializer.Deserialize<ConfigSpike.Config.SqlToGraphiteConfig>(xml);
                foreach (var job in sqlToGraphiteConfig.Jobs)
                {
                    Console.WriteLine(job.Type);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Console.Read();
        }
    }
}