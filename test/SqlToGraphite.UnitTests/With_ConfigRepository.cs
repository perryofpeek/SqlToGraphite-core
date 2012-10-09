using System;
using System.Collections.Generic;
using ConfigSpike.Config;
using log4net;
using NUnit.Framework;
using Rhino.Mocks;
using SqlToGraphite.Clients;
using SqlToGraphite.Conf;
using SqlToGraphite.Plugin.SqlServer;

namespace SqlToGraphite.UnitTests
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class With_ConfigRepository
    {
        private const string End = "</SqlToGraphiteConfig>";
        private const string Blank = "<?xml version='1.0' encoding='utf-8'?><SqlToGraphiteConfig></SqlToGraphiteConfig>";
        private const string TwoHosts = "<hosts><host name='DEFAULT'><role name='Default'/><role name='Default'/></host><host name='Server1'><role name='WebServer' /></host><host name='Server1'><role name='WebServer' /></host><host name='Server2'><role name='WebServer' /></host></hosts>";
        private const string TwoClients = "<Clients><Client name='statsdUdp' port='1234' /><Client name='graphitetcp' port='1234' /></Clients>";
        private const string UnknownClient = "<Clients><Client name='notknown' port='1234' /><Client name='graphitetcp' port='1234' /></Clients>";
        private const string Templates = " <templates><WorkItems Role='Default' ><TaskSet frequency='1000'><Task  path='some.path'  sql='sql'  connectionstring='cs'  type='type'  name='name' client='client' port='12345'/><Task  path='some.path'  sql='sql'  connectionstring='cs'  type='type'  name='name' client='client' port='12345'/></TaskSet><TaskSet frequency='5000'><Task  path='some.path'  sql='sql'  connectionstring='cs'  type='type'  name='name' client='client' port='12345'/><Task  path='some.path'  sql='sql'  connectionstring='cs'  type='type'  name='name' client='client' port='12345'/></TaskSet></WorkItems><WorkItems Role='WebServer' ><TaskSet frequency='1000'> <Task  path='some.path'  sql='sql'  connectionstring='cs'  type='type'  name='name' client='client' port='12345'/><Task  path='some.path'  sql='sql'  connectionstring='cs'  type='type'  name='name' client='client' port='12345'/></TaskSet> <TaskSet frequency='5000'><Task  path='some.path'  sql='sql'  connectionstring='cs'  type='type'  name='name' client='client' port='12345'/><Task  path='some.path'  sql='sql'  connectionstring='cs'  type='type'  name='name' client='client' port='12345'/></TaskSet></WorkItems></templates>";

        private IConfigReader reader;
        private ICache cache;
        private ISleep sleep;
        private ILog log;
        private ConfigRepository repository;
        private IConfigPersister configPersister;

        private int sleepTime;

        private IGenericSerializer genericSerializer;

        private SqlToGraphiteConfig config;

        [SetUp]
        public void SetUp()
        {
            sleepTime = 1000;
            config = new SqlToGraphiteConfig();
            reader = MockRepository.GenerateMock<IConfigReader>();
            cache = MockRepository.GenerateMock<ICache>();
            sleep = MockRepository.GenerateMock<ISleep>();
            log = MockRepository.GenerateMock<ILog>();
            configPersister = MockRepository.GenerateMock<IConfigPersister>();
            genericSerializer = MockRepository.GenerateMock<IGenericSerializer>();
            repository = new ConfigRepository(this.reader, new KnownGraphiteClients(), this.cache, this.sleep, this.log, sleepTime, configPersister, genericSerializer);
        }

        private string Add(string input, string data)
        {
            return input.Replace(End, string.Format("{0}{1}", data, End));
        }

        [Test]
        public void Should_save_config()
        {            
            var configXml = this.Add(Blank, TwoClients);
            cache.Expect(x => x.HasExpired()).Return(true).Repeat.Once();
            reader.Expect(x => x.GetXml()).Return(configXml);
            this.AddTwoClientsToConfig();
            genericSerializer.Expect(x => x.Deserialize<SqlToGraphiteConfig>(configXml)).Return(config);            
            configPersister.Expect(x => x.Save(config));
            repository.Load();
            //Act 
            repository.Save();
            //Assert.
            configPersister.VerifyAllExpectations();
            genericSerializer.VerifyAllExpectations();
        }

        [Test]
        public void Should_read_config_and_get_clients()
        {
            var configXml = this.Add(Blank, TwoClients);
            this.AddTwoClientsToConfig();
            genericSerializer.Expect(x => x.Deserialize<SqlToGraphiteConfig>(configXml)).Return(config);
            reader.Expect(x => x.GetXml()).Return(configXml);
            cache.Expect(x => x.HasExpired()).Return(true).Repeat.Once();
            cache.Expect(x => x.ResetCache()).Repeat.Once();
            repository.Load();
            Assert.That(repository.GetClients().Count, Is.EqualTo(2));
            reader.VerifyAllExpectations();
            cache.VerifyAllExpectations();
            genericSerializer.VerifyAllExpectations();
        }

        [Test]
        public void Should_retry_to_read_config_on_getting_error()
        {
            var configXml = this.Add(Blank, TwoClients);
            this.AddTwoClientsToConfig();
            genericSerializer.Expect(x => x.Deserialize<SqlToGraphiteConfig>(configXml)).Return(config);
            reader.Expect(x => x.GetXml()).Throw(new ApplicationException()).Repeat.Once();
            reader.Expect(x => x.GetXml()).Return(configXml).Repeat.Once();
            cache.Expect(x => x.HasExpired()).Return(true).Repeat.Once();
            cache.Expect(x => x.ResetCache()).Repeat.Once();
            sleep.Expect(x => x.Sleep(sleepTime));
            repository.Load();
            Assert.That(repository.GetClients().Count, Is.EqualTo(2));
            reader.VerifyAllExpectations();
            cache.VerifyAllExpectations();
            sleep.VerifyAllExpectations();
            genericSerializer.VerifyAllExpectations();
        }

        [Test]
        public void Should_load_config_and_get_clients()
        {
            this.AddTwoClientsToConfig();
            string configXml = this.Add(Blank, TwoClients);
            genericSerializer.Expect(x => x.Deserialize<SqlToGraphiteConfig>(configXml)).Return(config);
            reader.Expect(x => x.GetXml()).Return(configXml);
            cache.Expect(x => x.HasExpired()).Return(true).Repeat.Once();
            repository.Load();
            Assert.That(repository.GetClients().Count, Is.EqualTo(2));
            reader.VerifyAllExpectations();
            cache.VerifyAllExpectations();
        }

        private void AddTwoClientsToConfig()
        {
            var c1 = new Config.GraphiteTcpClient { ClientName = "ClientName" };
            this.config.Clients.Add(c1);
            this.config.Clients.Add(new ConfigSpike.GraphiteUdpClient());
        }

        [Test]
        public void Should_add_clients()
        {
            repository.AddClient(new Config.GraphiteTcpClient { ClientName = "abc", Port = 123 });
            var clients = repository.GetClients();
            Assert.That(clients.Count, Is.EqualTo(1));
        }

        [Test]
        public void Should_add_host()
        {
            var role1 = "a1";
            var role2 = "a2";

            var name = Guid.NewGuid().ToString();
            var roles = new List<Role> { new Role() { Name = role1 }, new Role() { Name = role2 } };
            //Test
            repository.AddHost(name, roles);
            //Assert
            var hosts = repository.GetHosts();
            Assert.That(hosts.Count, Is.EqualTo(1));
            Assert.That(hosts[0].Name, Is.EqualTo(name));
            Assert.That(hosts[0].Roles[0].Name, Is.EqualTo(role1));
            Assert.That(hosts[0].Roles[1].Name, Is.EqualTo(role2));
        }

        [Test]
        public void AddNewJob()
        {
            var jobName = "job1";
            var job = new SqlServerClient();
            job.Name = jobName;
            job.ClientName = "TcpGraphite";
            repository.AddJob(job);
            var jobs = repository.GetJobs();
            Assert.That(jobs.Count, Is.EqualTo(1));
        }

        [Test]
        public void Should_add_task_to_role()
        {
            var roleName = "someRole";
            var frequency = 1000;
            var jobName = "jobName";
            var t = new TaskDetails(roleName, frequency, jobName);

            //Test
            repository.AddTask(t);
            //Assert
            var templates = repository.GetTemplates();
            Assert.That(templates.Count, Is.EqualTo(1));
            Assert.That(templates[0].WorkItems[0].RoleName, Is.EqualTo(t.Role));
            Assert.That(templates[0].WorkItems[0].TaskSet[0].Frequency, Is.EqualTo(t.Frequency));
            Assert.That(templates[0].WorkItems[0].TaskSet[0].Tasks[0].JobName, Is.EqualTo(jobName));
        }

        private static void AssertThatTaskEqualToTaskProperty(TaskDetails t, ConfigSpike.Config.Task task)
        {
            Assert.Fail("commented out shit");
            //Assert.That(task.client, Is.EqualTo(t.Client));
            //Assert.That(task.connectionstring, Is.EqualTo(t.Connectionstring));
            //Assert.That(task.name, Is.EqualTo(t.Name));
            //Assert.That(task.path, Is.EqualTo(t.Path));
            //Assert.That(task.port, Is.EqualTo(t.Port));
            //Assert.That(task.sql, Is.EqualTo(t.Sql));
            //Assert.That(task.type, Is.EqualTo(t.Type));
        }

        [Test]
        public void Should_add_task_to_new_role()
        {
            var t0 = new TaskDetails("someRole1", 1000, "job1");
            var t1 = new TaskDetails("someRole2", 1000, "client");
            //Test
            repository.AddTask(t0);
            repository.AddTask(t1);
            //Assert
            var templates = repository.GetTemplates();
            Assert.That(templates[0].WorkItems.Count, Is.EqualTo(2));

            Assert.That(templates[0].WorkItems[0].RoleName, Is.EqualTo(t0.Role));
            Assert.That(templates[0].WorkItems[0].TaskSet[0].Frequency, Is.EqualTo(t0.Frequency));
            Assert.That(templates[0].WorkItems[0].TaskSet[0].Tasks[0].JobName, Is.EqualTo(t0.JobName));
            Assert.That(templates[0].WorkItems[1].RoleName, Is.EqualTo(t1.Role));
            Assert.That(templates[0].WorkItems[1].TaskSet[0].Frequency, Is.EqualTo(t1.Frequency));
            Assert.That(templates[0].WorkItems[1].TaskSet[0].Tasks[0].JobName, Is.EqualTo(t1.JobName));
        }

        [Test]
        public void Should_add_second_task_to_same_role_different_frequency()
        {
            var t0 = new TaskDetails("someRole1", 1000, "job1");
            var t1 = new TaskDetails("someRole2", 2000, "client");
            //Test
            repository.AddTask(t0);
            repository.AddTask(t1);
            //Assert
            var templates = repository.GetTemplates();
            Assert.That(templates.Count, Is.EqualTo(1));
            Assert.That(templates[0].WorkItems[0].RoleName, Is.EqualTo(t0.Role));
            Assert.That(templates[0].WorkItems[0].TaskSet[0].Frequency, Is.EqualTo(t0.Frequency));
            Assert.That(templates[0].WorkItems[0].TaskSet[0].Tasks[0].JobName, Is.EqualTo(t0.JobName));
            Assert.That(templates[0].WorkItems[1].RoleName, Is.EqualTo(t1.Role));
            Assert.That(templates[0].WorkItems[1].TaskSet[0].Frequency, Is.EqualTo(t1.Frequency));
            Assert.That(templates[0].WorkItems[1].TaskSet[0].Tasks[0].JobName, Is.EqualTo(t1.JobName));
        }

        [Test]
        public void Should_add_second_task_to_same_role_same_frequency()
        {
            var t0 = new TaskDetails("someRole1", 1000, "job1");
            var t1 = new TaskDetails("someRole1", 1000, "client");
            //Test
            repository.AddTask(t0);
            repository.AddTask(t1);
            //Assert
            var templates = repository.GetTemplates();
            Assert.That(templates.Count, Is.EqualTo(1));
            Assert.That(templates[0].WorkItems[0].RoleName, Is.EqualTo(t0.Role));
            Assert.That(templates[0].WorkItems[0].TaskSet[0].Frequency, Is.EqualTo(t0.Frequency));
            Assert.That(templates[0].WorkItems[0].TaskSet[0].Tasks[0].JobName, Is.EqualTo(t0.JobName));
            Assert.That(templates[0].WorkItems[0].TaskSet[0].Tasks[1].JobName, Is.EqualTo(t1.JobName));
        }

        [Test]
        public void Should_read_config_and_get_clients_dictionary()
        {
            string namec1 = "c1Name";
            string namec2 = "c2Name";

            config.Clients.Add(new Config.GraphiteTcpClient { ClientName = namec1 });
            config.Clients.Add(new ConfigSpike.GraphiteUdpClient { ClientName = namec2 });
            string configXml = this.Add(Blank, TwoClients);
            genericSerializer.Expect(x => x.Deserialize<SqlToGraphiteConfig>(configXml)).Return(config);
            reader.Expect(x => x.GetXml()).Return(configXml);
            cache.Expect(x => x.HasExpired()).Return(true).Repeat.Once();
            //Test
            repository.Load();
            //Assert
            Assert.That(repository.GetClients().Count, Is.EqualTo(2));
            reader.VerifyAllExpectations();
            cache.VerifyAllExpectations();
            genericSerializer.VerifyAllExpectations();
        }

        //I don't this this is relevant with plug-in clients. 
        //[Test]
        //public void Should_read_config_and_get_error_with_clients_dictionary()
        //{
        //    var config = new SqlToGraphiteConfig();
        //    string configXml = this.Add(Blank, UnknownClient);
        //    genericSerializer.Expect(x => x.Deserialize<SqlToGraphiteConfig>(configXml)).Return(config).Repeat.Once();
        //    reader.Expect(x => x.GetXml()).Return(configXml);
        //    cache.Expect(x => x.HasExpired()).Return(true).Repeat.Once();
        //    repository.Load();
        //    Assert.That(repository.Errors.Count, Is.EqualTo(1));
        //    Assert.That(repository.Errors[0].Contains(ConfigRepository.UnknownClient), Is.EqualTo(true));
        //    Assert.That(repository.Validate(), Is.EqualTo(false));
        //    reader.VerifyAllExpectations();
        //    cache.VerifyAllExpectations();
        //    genericSerializer.VerifyAllExpectations();
        //}
        [Test]
        public void Should_read_config_and_get_Templates()
        {
            config.Templates.Add(new Template());
            config.Templates.Add(new Template());
            string configXml = this.Add(Blank, Templates);
            genericSerializer.Expect(x => x.Deserialize<SqlToGraphiteConfig>(configXml)).Return(config).Repeat.Once();
            reader.Expect(x => x.GetXml()).Return(configXml);
            cache.Expect(x => x.HasExpired()).Return(true).Repeat.Once();
            //Test
            repository.Load();
            //Assert
            Assert.That(repository.GetTemplates().Count, Is.EqualTo(2));
            reader.VerifyAllExpectations();
            cache.VerifyAllExpectations();
            genericSerializer.VerifyAllExpectations();
        }

        [Test]
        public void Should_read_config_from_cache()
        {
            config.Templates.Add(new Template());
            config.Templates.Add(new Template());
            string configXml = this.Add(Blank, Templates);
            genericSerializer.Expect(x => x.Deserialize<SqlToGraphiteConfig>(configXml)).Return(config).Repeat.Once();
            reader.Expect(x => x.GetXml()).Return(configXml).Repeat.Once();
            cache.Expect(x => x.HasExpired()).Return(true).Repeat.Once();
            cache.Expect(x => x.HasExpired()).Return(false).Repeat.Once();
            //Test
            repository.Load();
            repository.Load();
            //Assert
            Assert.That(repository.GetTemplates().Count, Is.EqualTo(2));
            reader.VerifyAllExpectations();
            cache.VerifyAllExpectations();
            genericSerializer.VerifyAllExpectations();
        }

        [Test]
        public void Should_read_config_and_get_Hosts()
        {
            string configXml = this.Add(Blank, TwoHosts);
            config.Hosts.Add(new Host());
            config.Hosts.Add(new Host());
            genericSerializer.Expect(x => x.Deserialize<SqlToGraphiteConfig>(configXml)).Return(config);
            reader.Expect(x => x.GetXml()).Return(configXml);
            cache.Expect(x => x.HasExpired()).Return(true).Repeat.Once();
            //Test
            repository.Load();
            //Assert
            Assert.That(repository.GetHosts().Count, Is.EqualTo(2));
            reader.VerifyAllExpectations();
            cache.VerifyAllExpectations();
            genericSerializer.VerifyAllExpectations();
        }

        [Test]
        public void Should_not_validate_because_templates_are_not_defined()
        {
            string configXml = this.Add(Add(Blank, TwoClients), TwoHosts);
            genericSerializer.Expect(x => x.Deserialize<SqlToGraphiteConfig>(configXml)).Return(new SqlToGraphiteConfig());
            reader.Expect(x => x.GetXml()).Return(configXml);
            cache.Expect(x => x.HasExpired()).Return(true).Repeat.Once();
            //Test
            repository.Load();
            //Assert
            Assert.That(repository.Validate(), Is.EqualTo(false));
            Assert.That(repository.Errors.Contains(ConfigRepository.FailedToFindTemplates), Is.EqualTo(true));
            reader.VerifyAllExpectations();
            cache.VerifyAllExpectations();
            genericSerializer.VerifyAllExpectations();
        }

        [Test]
        public void Should_not_validate_because_hosts_are_not_defined()
        {
            string configXml = this.Add(Add(Blank, TwoClients), Templates);
            genericSerializer.Expect(x => x.Deserialize<SqlToGraphiteConfig>(configXml)).Return(new SqlToGraphiteConfig());
            reader.Expect(x => x.GetXml()).Return(configXml);
            cache.Expect(x => x.HasExpired()).Return(true).Repeat.Once();
            //Test
            repository.Load();
            //Assert
            Assert.That(repository.Validate(), Is.EqualTo(false));
            Assert.That(repository.Errors.Contains(ConfigRepository.FailedToFindHosts), Is.EqualTo(true));
            reader.VerifyAllExpectations();
            cache.VerifyAllExpectations();
            genericSerializer.VerifyAllExpectations();
        }

        [Test]
        public void Should_not_validate_because_clients_are_not_defined()
        {
            string configXml = this.Add(Add(Blank, TwoHosts), Templates);
            genericSerializer.Expect(x => x.Deserialize<SqlToGraphiteConfig>(configXml)).Return(new SqlToGraphiteConfig());
            reader.Expect(x => x.GetXml()).Return(configXml);
            cache.Expect(x => x.HasExpired()).Return(true).Repeat.Once();
            //Test
            repository.Load();
            //Assert
            Assert.That(repository.Validate(), Is.EqualTo(false));
            Assert.That(repository.Errors.Contains(ConfigRepository.FailedToFindClients), Is.EqualTo(true));
            reader.VerifyAllExpectations();
            cache.VerifyAllExpectations();
            genericSerializer.VerifyAllExpectations();
        }

        [Test]
        public void Should_validate_sucessfully()
        {
            string configXml = this.Add(Add(Add(Blank, TwoHosts), Templates), TwoClients);
            genericSerializer.Expect(x => x.Deserialize<SqlToGraphiteConfig>(configXml)).Return(config);
            this.AddTwoClientsToConfig();
                                 
            reader.Expect(x => x.GetXml()).Return(configXml);
            cache.Expect(x => x.HasExpired()).Return(true).Repeat.Once();
            //Test
            repository.Load();

            var job = new SqlServerClient();
            job.Name = "fred";
            job.ClientName = "ClientName";
            repository.AddJob(job);
            repository.AddHost("hostname", new List<Role> { new Role { Name = "roleName" } });
            repository.AddTask(new TaskDetails("roleName", 1000, "fred"));  
            
            //Assert          
            Assert.That(repository.Validate(), Is.EqualTo(true));
            Assert.That(repository.Errors.Count, Is.EqualTo(0));
            reader.VerifyAllExpectations();
            cache.VerifyAllExpectations();
            genericSerializer.VerifyAllExpectations();
        }

        [Test]
        public void Should_return_failed_to_load_config_error()
        {
            var configXml = this.Add(Blank, TwoClients);
            this.AddTwoClientsToConfig();
            genericSerializer.Expect(x => x.Deserialize<SqlToGraphiteConfig>(configXml)).Return(new SqlToGraphiteConfig());
            reader.Expect(x => x.GetXml()).Return(Blank).Repeat.Twice();
            reader.Expect(x => x.GetXml()).Return(configXml).Repeat.Once();
            sleep.Expect(x => x.Sleep(sleepTime)).Repeat.Twice();
            cache.Expect(x => x.HasExpired()).Return(true);
            repository.Load();
            //Assert.That(repository.Errors.Count, Is.EqualTo(1));
            //Assert.That(repository.Validate(), Is.EqualTo(false));
            //Assert.That(repository.Errors.Count, Is.EqualTo(4));
            //Assert.That(repository.Errors[0], Is.EqualTo(ConfigRepository.FailedToLoadAnyConfiguration));
            reader.VerifyAllExpectations();
            cache.VerifyAllExpectations();
            sleep.VerifyAllExpectations();
            genericSerializer.VerifyAllExpectations();
        }
    }
}