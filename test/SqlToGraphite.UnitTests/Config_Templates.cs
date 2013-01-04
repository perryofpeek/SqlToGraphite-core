namespace SqlToGraphite.UnitTests
{
    using log4net;
    using NUnit.Framework;
    using Rhino.Mocks;
    using SqlToGraphite;
    using SqlToGraphite.Config;
    using SqlToGraphite.Plugin.Wmi;

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

        private ILog log;

        [SetUp]
        public void SetUp()
        {
            this.log = MockRepository.GenerateMock<ILog>();
            this.assemblyResolver = new AssemblyResolver(new DirectoryImpl(), log);
            this.config = new SqlToGraphiteConfig(this.assemblyResolver, log);
            this.template = new Template();
        }

        [Test]
        public void Should_validate_if_job_does_not_exist()
        {
            this.template.WorkItems.Add(CreateWorkItems(this.jobName1, this.roleName1, this.freq1));
            this.config.Templates.Add(this.template);
            log.Expect(x => x.Error(string.Format("The job named {0} has not been defined for the task in role {1}", this.jobName1, this.roleName1)));

            this.config.Validate();
            //Test
            log.VerifyAllExpectations();
        }

        //Freq Cannot be 0 or less 
        [Test]
        public void Should_validate_job_if_client_exist()
        {
            var name = "Name";
            var clientName = "SomeClient";
            var c = new GraphiteTcpClient { ClientName = clientName };
            this.config.Clients.Add(c);
            this.config.Jobs.Add(new WmiClient { ClientName = clientName, Name = name });
            //Test
            this.config.Validate();
        }

        [Test]
        public void Should_add_single_task()
        {
            this.template.WorkItems.Add(CreateWorkItems(this.jobName1, this.roleName1, this.freq1));
            this.config.Templates.Add(this.template);
            //Test
            var sqlToGraphiteConfig = Helper.SerialiseDeserialise(this.config);
            //Assert
            Assert.That(sqlToGraphiteConfig.Templates[0].WorkItems[0].RoleName, Is.EqualTo(this.roleName1));
            Assert.That(sqlToGraphiteConfig.Templates[0].WorkItems[0].TaskSet[0].Tasks[0].JobName, Is.EqualTo(this.jobName1));
            Assert.That(sqlToGraphiteConfig.Templates[0].WorkItems[0].TaskSet[0].Frequency, Is.EqualTo(this.freq1));
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
            this.template.WorkItems.Add(CreateWorkItems(this.jobName1, this.roleName1, this.freq1));
            this.template.WorkItems.Add(CreateWorkItems(this.taskName2, this.roleName2, this.freq2));
            this.config.Templates.Add(this.template);
            //Test
            var sqlToGraphiteConfig = Helper.SerialiseDeserialise(this.config);
            //Assert
            Assert.That(sqlToGraphiteConfig.Templates[0].WorkItems[0].RoleName, Is.EqualTo(this.roleName1));
            Assert.That(sqlToGraphiteConfig.Templates[0].WorkItems[0].TaskSet[0].Tasks[0].JobName, Is.EqualTo(this.jobName1));
            Assert.That(sqlToGraphiteConfig.Templates[0].WorkItems[0].TaskSet[0].Frequency, Is.EqualTo(this.freq1));

            Assert.That(sqlToGraphiteConfig.Templates[0].WorkItems[1].RoleName, Is.EqualTo(this.roleName1));
            Assert.That(sqlToGraphiteConfig.Templates[0].WorkItems[1].TaskSet[0].Tasks[0].JobName, Is.EqualTo(this.jobName1));
            Assert.That(sqlToGraphiteConfig.Templates[0].WorkItems[1].TaskSet[0].Frequency, Is.EqualTo(this.freq1));
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