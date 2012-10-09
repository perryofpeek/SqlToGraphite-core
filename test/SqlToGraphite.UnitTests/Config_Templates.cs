using ConfigSpike.Config;
using NUnit.Framework;

using SqlToGraphite;
using SqlToGraphite.Plugin.SqlServer;
using SqlToGraphite.UnitTests;

using GraphiteTcpClient = SqlToGraphite.Config.GraphiteTcpClient;

namespace ConfigSpike
{
    [TestFixture]
    // ReSharper disable InconsistentNaming
    public class Config_Templates
    {
        private string roleName1 = "roleName";
        private string jobName1 = "taskName";
        private int freq1 = 100;

        private string roleName2 = "roleName";
        private string taskName2 = "taskName";
        private int freq2 = 100;

        private SqlToGraphiteConfig config;
        private Template template;        

        private IAssemblyResolver assemblyResolver;    

        [SetUp]
        public void SetUp()
        {
            assemblyResolver = new AssemblyResolver(new DirectoryImpl());
            config = new SqlToGraphiteConfig(assemblyResolver);
            template = new Template();
        }

        [Test]
        public void Should_not_validate_if_role_does_not_exist()
        {
            template.WorkItems.Add(CreateWorkItems(jobName1, roleName1, freq1));
            config.Templates.Add(template);

            var ex = Assert.Throws<JobNotDefinedForTaskException>(() => config.Validate());
            //Test
            Assert.That(ex.Message, Is.EqualTo(string.Format("The job named {0} has not been defined for the task in role {1}", jobName1, roleName1)));
        }

        //Freq Cannot be 0 or less 
        [Test]
        public void Should_validate_job_if_client_exist()
        {
            var name = "Name";
            var clientName = "SomeClient";            
            var c = new GraphiteTcpClient { ClientName = clientName };
            config.Clients.Add(c);
            config.Jobs.Add(new SqlServerClient { ClientName = clientName, Name = name });
            //Test
            config.Validate();
        }

        [Test]
        public void Should_add_single_task()
        {
            template.WorkItems.Add(CreateWorkItems(this.jobName1, roleName1, freq1));
            config.Templates.Add(template);
            //Test
            var sqlToGraphiteConfig = Helper.SerialiseDeserialise(config);
            //Assert
            Assert.That(sqlToGraphiteConfig.Templates[0].WorkItems[0].RoleName, Is.EqualTo(roleName1));
            Assert.That(sqlToGraphiteConfig.Templates[0].WorkItems[0].TaskSet[0].Tasks[0].JobName, Is.EqualTo(this.jobName1));
            Assert.That(sqlToGraphiteConfig.Templates[0].WorkItems[0].TaskSet[0].Frequency, Is.EqualTo(freq1));
        }

        private static WorkItems CreateWorkItems(string taskName, string roleName, int freq)
        {
            var wi = new WorkItems { RoleName = roleName };
            var ts = new TaskSet { Frequency = freq };
            var task = new Task { JobName = taskName };
            ts.Tasks.Add(task);
            wi.TaskSet.Add(ts);
            return wi;
        }

        [Test]
        public void Should_add_multiple_taskset_of_same_type()
        {
            template.WorkItems.Add(CreateWorkItems(this.jobName1, roleName1, freq1));
            template.WorkItems.Add(CreateWorkItems(taskName2, roleName2, freq2));
            config.Templates.Add(template);
            //Test
            var sqlToGraphiteConfig = Helper.SerialiseDeserialise(config);
            //Assert
            Assert.That(sqlToGraphiteConfig.Templates[0].WorkItems[0].RoleName, Is.EqualTo(roleName1));
            Assert.That(sqlToGraphiteConfig.Templates[0].WorkItems[0].TaskSet[0].Tasks[0].JobName, Is.EqualTo(this.jobName1));
            Assert.That(sqlToGraphiteConfig.Templates[0].WorkItems[0].TaskSet[0].Frequency, Is.EqualTo(freq1));

            Assert.That(sqlToGraphiteConfig.Templates[0].WorkItems[1].RoleName, Is.EqualTo(roleName1));
            Assert.That(sqlToGraphiteConfig.Templates[0].WorkItems[1].TaskSet[0].Tasks[0].JobName, Is.EqualTo(this.jobName1));
            Assert.That(sqlToGraphiteConfig.Templates[0].WorkItems[1].TaskSet[0].Frequency, Is.EqualTo(freq1));
        }

        //[Test]
        //public void Should_add_multiple_jobs_of_different_types()
        //{
        //    var name1 = "Name1";
        //    var client1 = "SomeClient1";
        //    var name2 = "Name2";
        //    var client2 = "SomeClient2";

        //    var config = new SqlToGraphiteConfig();
        //    config.Jobs.Add(new SqlServer { ClientName = client1, Name = name1 });
        //    config.Jobs.Add(new WmiPlugin { ClientName = client2, Name = name2 });
        //    //Test
        //    var sqlToGraphiteConfig = Helper.SerialiseDeserialise(config);
        //    //Assert
        //    Assert.That(sqlToGraphiteConfig.Jobs[0].ClientName, Is.EqualTo(client1));
        //    Assert.That(sqlToGraphiteConfig.Jobs[0].Name, Is.EqualTo(name1));
        //    Assert.That(sqlToGraphiteConfig.Jobs[0], Is.TypeOf<SqlServer>());
        //    Assert.That(sqlToGraphiteConfig.Jobs[1].ClientName, Is.EqualTo(client2));
        //    Assert.That(sqlToGraphiteConfig.Jobs[1].Name, Is.EqualTo(name2));
        //    Assert.That(sqlToGraphiteConfig.Jobs[1], Is.TypeOf<WmiPlugin>());
        //}
    }
}