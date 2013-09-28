using System;
using System.Collections.Generic;
using log4net;
using NUnit.Framework;
using SqlToGraphite.Config;
using SqlToGraphiteInterfaces;
using SqlToGraphite.Plugin.Wmi;

namespace SqlToGraphite.UnitTests
{
    // ReSharper disable InconsistentNaming
 
    [TestFixture]
    public class Config_generation
    {
        [Test]
        public void Should_generate_test_config_file()
        {
            try
            {
                var log = LogManager.GetLogger("log");
                var encryption = new Encryption();
                var simpleCs = encryption.Encrypt("some Cs");

                log4net.Config.XmlConfigurator.Configure();


                var job1 = new WmiClient { ClientName = "GraphiteTcpClient", Name = "GetNumberOfTransactionsASecond", Hostname = simpleCs, Sql = "some sql" };
                var job2 = new WmiClient { ClientName = "GraphiteUdpClient", Name = "GetNumberOfDeliveryies", Hostname = simpleCs, Sql = "some sql1" };
                var client1 = new GraphiteTcpClient { ClientName = "GraphiteTcpClient", Port = 2003, Hostname = "metrics.london.ttldev.local" };
                var client2 = new GraphiteUdpClient { ClientName = "GraphiteUdpClient", Port = 2003, Hostname = "metrics.london.ttldev.local" };

                var config = new SqlToGraphiteConfig(new AssemblyResolver(new DirectoryImpl(), log), log);
                config.Jobs.Add(job1);
                config.Jobs.Add(job2);

                var host1 = new Host { Name = "TTL001121" };
                var host2 = new Host { Name = "Server1" };
                var role1 = new Role { Name = "ProductionTests" };
                var role2 = new Role { Name = "default" };
                var role3 = new Role { Name = "SqlTests" };

                host1.Roles.Add(role1);
                host1.Roles.Add(role2);
                host2.Roles.Add(role2);
                host2.Roles.Add(role3);
                config.Hosts.Add(host1);
                config.Hosts.Add(host2);

                var template = new Template();
                var wi = new WorkItems { RoleName = role1.Name, TaskSet = new List<TaskSet>() };
                var taskSet = new TaskSet { Frequency = 1000 };
                taskSet.Tasks.Add(new Task { JobName = job1.Name });
                wi.TaskSet.Add(taskSet);
                template.WorkItems.Add(wi);

                var wi1 = new WorkItems { RoleName = role2.Name, TaskSet = new List<TaskSet>() };
                var taskSet1 = new TaskSet { Frequency = 2000 };
                taskSet1.Tasks.Add(new Task { JobName = job2.Name });
                wi1.TaskSet.Add(taskSet1);
                template.WorkItems.Add(wi1);

                var wi2 = new WorkItems { RoleName = role3.Name, TaskSet = new List<TaskSet>() };
                var taskSet2 = new TaskSet { Frequency = 3000 };
                wi2.TaskSet.Add(taskSet2);
                template.WorkItems.Add(wi2);

                config.Templates.Add(template);
                config.Clients = new ListOfUniqueType<Client> { client1, client2 };
                //var genericSerializer = new GenericSerializer(Global.GetNameSpace());
                //string xml = genericSerializer.Serialize(config);
                //Console.WriteLine(xml);
                //var sqlToGraphiteConfig = genericSerializer.Deserialize<SqlToGraphiteConfig>(xml);
                //foreach (var job in sqlToGraphiteConfig.Jobs)
                //{
                    //Console.WriteLine(job.Type);
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}