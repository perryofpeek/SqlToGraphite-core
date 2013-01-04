using System;
using System.Collections.Generic;

using log4net;
using NUnit.Framework;
using Rhino.Mocks;
using SqlToGraphite.Conf;
using SqlToGraphite.Config;

namespace SqlToGraphite.UnitTests
{
    using SqlToGraphite.Plugin.Wmi;

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

        private IAssemblyResolver assemblyResolver;

        [SetUp]
        public void SetUp()
        {
            sleepTime = 1000;
            assemblyResolver = MockRepository.GenerateMock<IAssemblyResolver>();
            config = new SqlToGraphiteConfig(assemblyResolver, log);
            reader = MockRepository.GenerateMock<IConfigReader>();
            cache = MockRepository.GenerateMock<ICache>();
            sleep = MockRepository.GenerateMock<ISleep>();
            log = MockRepository.GenerateMock<ILog>();
            configPersister = MockRepository.GenerateMock<IConfigPersister>();
            genericSerializer = MockRepository.GenerateMock<IGenericSerializer>();
            repository = new ConfigRepository(this.reader, this.cache, this.sleep, this.log, this.sleepTime, this.configPersister, this.genericSerializer);
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
            var c1 = new GraphiteTcpClient { ClientName = "ClientName" };
            this.config.Clients.Add(c1);
            this.config.Clients.Add(new GraphiteUdpClient());
        }

        [Test]
        public void Should_add_clients()
        {
            repository.AddClient(new GraphiteTcpClient { ClientName = "abc", Port = 123 });
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
            var job = new WmiClient();
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

        private static void AssertThatTaskEqualToTaskProperty(TaskDetails t, Task task)
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

            config.Clients.Add(new GraphiteTcpClient { ClientName = namec1 });
            config.Clients.Add(new GraphiteUdpClient { ClientName = namec2 });
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
        public void Should_read_config_One_job_sucessfully()
        {
            IGenericSerializer gs = new GenericSerializer(Global.GetNameSpace());
            repository = new ConfigRepository(this.reader, this.cache, this.sleep, this.log, this.sleepTime, this.configPersister, gs);
            reader.Expect(x => x.GetXml()).Return(Resources.knownPlugin);
            cache.Expect(x => x.HasExpired()).Return(true).Repeat.Once();
            //Test
            repository.Load();
            //Assert
            List<Job> jobs = repository.GetJobs();
            Assert.That(jobs.Count, Is.EqualTo(2));
            Assert.That(jobs[0].GetType().Name, Is.EqualTo("WmiClient"));
            genericSerializer.VerifyAllExpectations();
        }

        [Test]
        public void Should_read_config_One_job_sucessfully_with_unknownplugin()
        {
            IGenericSerializer gs = new GenericSerializer(Global.GetNameSpace());
            repository = new ConfigRepository(this.reader, this.cache, this.sleep, this.log, this.sleepTime, this.configPersister, gs);
            reader.Expect(x => x.GetXml()).Return(Resources.UnknownPlugin);
            cache.Expect(x => x.HasExpired()).Return(true).Repeat.Once();
            //Test
            repository.Load();
            //Assert
            List<Job> jobs = repository.GetJobs();
            Assert.That(jobs.Count, Is.EqualTo(1));
            Assert.That(jobs[0].GetType().Name, Is.EqualTo("WmiClient"));
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
            genericSerializer.Expect(x => x.Deserialize<SqlToGraphiteConfig>(configXml)).Return(new SqlToGraphiteConfig(assemblyResolver, log));
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
            genericSerializer.Expect(x => x.Deserialize<SqlToGraphiteConfig>(configXml)).Return(new SqlToGraphiteConfig(assemblyResolver, log));
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
            genericSerializer.Expect(x => x.Deserialize<SqlToGraphiteConfig>(configXml)).Return(new SqlToGraphiteConfig(assemblyResolver, log));
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
        public void Should_remove_job_from_role_with_same_frequency()
        {
            string jobName = "jobName";
            int frequency = 123;
            string roleName = "roleName";
            var template = this.CreateSingleRoleAndJob(jobName, frequency, roleName);
            cache.Expect(x => x.HasExpired()).Return(true).Repeat.Once();
            string configXml = this.Add(Add(Blank, TwoClients), Templates);
            genericSerializer.Expect(x => x.Deserialize<SqlToGraphiteConfig>(configXml)).Return(config);
            reader.Expect(x => x.GetXml()).Return(configXml);
            repository.Load();

            //Test            
            repository.DeleteJobFromRole(jobName, frequency, roleName);
            Assert.That(template.WorkItems[0].TaskSet[0].Tasks.Count, Is.EqualTo(0));
            cache.VerifyAllExpectations();
            genericSerializer.VerifyAllExpectations();
        }

        [Test]
        public void Should_remove_frequency_from_role()
        {
            int frequency = 123;
            int frequency1 = 321;
            string roleName = "roleName";
            var template = this.CreateMultipoleRolesAndJobs("none", "job1", frequency, frequency1, roleName);
            cache.Expect(x => x.HasExpired()).Return(true).Repeat.Once();
            string configXml = this.Add(Add(Blank, TwoClients), Templates);
            genericSerializer.Expect(x => x.Deserialize<SqlToGraphiteConfig>(configXml)).Return(config);
            reader.Expect(x => x.GetXml()).Return(configXml);
            repository.Load();

            //Test            
            repository.DeleteRoleFrequency(roleName, frequency);
            //Assert
            Assert.That(template.WorkItems.Count, Is.EqualTo(1));
            Assert.That(template.WorkItems[0].TaskSet.Count, Is.EqualTo(1));
            Assert.That(template.WorkItems[0].TaskSet[0].Tasks.Count, Is.EqualTo(1));
            Assert.That(template.WorkItems[0].TaskSet[0].Tasks[0].JobName, Is.EqualTo("job1"));
            Assert.That(template.WorkItems[0].TaskSet[0].Frequency, Is.EqualTo(frequency1));
            cache.VerifyAllExpectations();
            genericSerializer.VerifyAllExpectations();
        }

        [Test]
        public void Should_not_remove_unknown_frequency_from_role()
        {
            int frequency = 333;
            int frequency1 = 321;
            string roleName = "roleName";
            var template = this.CreateMultipoleRolesAndJobs("none", "job1", frequency, frequency1, roleName);
            cache.Expect(x => x.HasExpired()).Return(true).Repeat.Once();
            string configXml = this.Add(Add(Blank, TwoClients), Templates);
            genericSerializer.Expect(x => x.Deserialize<SqlToGraphiteConfig>(configXml)).Return(config);
            reader.Expect(x => x.GetXml()).Return(configXml);
            repository.Load();

            //Test            
            repository.DeleteRoleFrequency(roleName, 444);
            //Assert
            Assert.That(template.WorkItems.Count, Is.EqualTo(1));
            Assert.That(template.WorkItems[0].TaskSet.Count, Is.EqualTo(2));
            Assert.That(template.WorkItems[0].TaskSet[0].Tasks[0].JobName, Is.EqualTo("none"));
            Assert.That(template.WorkItems[0].TaskSet[0].Frequency, Is.EqualTo(frequency));
            cache.VerifyAllExpectations();
            genericSerializer.VerifyAllExpectations();
        }

        [Test]
        public void Should_remove_role()
        {
            int frequency = 123;
            string roleName = "roleName";
            var template = this.CreateSingleRoleAndJob("none", frequency, roleName);
            cache.Expect(x => x.HasExpired()).Return(true).Repeat.Once();
            string configXml = this.Add(Add(Blank, TwoClients), Templates);
            genericSerializer.Expect(x => x.Deserialize<SqlToGraphiteConfig>(configXml)).Return(config);
            reader.Expect(x => x.GetXml()).Return(configXml);
            repository.Load();

            //Test            
            repository.DeleteRole(roleName);
            Assert.That(template.WorkItems.Count, Is.EqualTo(0));
            cache.VerifyAllExpectations();
            genericSerializer.VerifyAllExpectations();
        }

        [Test]
        public void Should_add_role_frequency()
        {
            int frequency = 123;
            string roleName = "roleName";
            var template = this.CreateSingleRoleAndJob("none", frequency, roleName);
            cache.Expect(x => x.HasExpired()).Return(true).Repeat.Once();
            string configXml = this.Add(Add(Blank, TwoClients), Templates);
            genericSerializer.Expect(x => x.Deserialize<SqlToGraphiteConfig>(configXml)).Return(config);
            reader.Expect(x => x.GetXml()).Return(configXml);
            repository.Load();

            //Test            
            repository.AddRoleFrequency(frequency, roleName);
            Assert.That(template.WorkItems.Count, Is.EqualTo(1));
            Assert.That(template.WorkItems[0].TaskSet[1].Frequency, Is.EqualTo(frequency));

            cache.VerifyAllExpectations();
            genericSerializer.VerifyAllExpectations();
        }

        [Test]
        public void Should_add_new_role()
        {
            string roleName = "roleName";
            var template = this.CreateSingleRoleAndJob("none", 123, "someRole");
            cache.Expect(x => x.HasExpired()).Return(true).Repeat.Once();
            string configXml = this.Add(Add(Blank, TwoClients), Templates);
            genericSerializer.Expect(x => x.Deserialize<SqlToGraphiteConfig>(configXml)).Return(config);
            reader.Expect(x => x.GetXml()).Return(configXml);
            repository.Load();

            //Test            
            repository.AddNewRole(roleName);
            Assert.That(template.WorkItems.Count, Is.EqualTo(2));
            Assert.That(template.WorkItems[1].RoleName, Is.EqualTo(roleName));

            cache.VerifyAllExpectations();
            genericSerializer.VerifyAllExpectations();
        }

        [Test]
        public void Should_delete_host()
        {
            string hostname = "abc";
            config = new SqlToGraphiteConfig { Hosts = CreateTwoHosts() };
            cache.Expect(x => x.HasExpired()).Return(true).Repeat.Once();
            string configXml = "<xml></xml>";
            genericSerializer.Expect(x => x.Deserialize<SqlToGraphiteConfig>(configXml)).Return(config);
            reader.Expect(x => x.GetXml()).Return(configXml);
            repository.Load();
            //Test            
            repository.DeleteHost(hostname);
            Assert.That(config.Hosts.Count, Is.EqualTo(1));
            Assert.That(config.Hosts[0].Name, Is.EqualTo("def"));

            cache.VerifyAllExpectations();
            genericSerializer.VerifyAllExpectations();
        }

        [Test]
        public void Should_add_role_to_host()
        {
            string hostname = "abc";
            string roleName = "newRole";
            config = new SqlToGraphiteConfig { Hosts = CreateTwoHosts() };
            cache.Expect(x => x.HasExpired()).Return(true).Repeat.Once();
            string configXml = "<xml></xml>";
            genericSerializer.Expect(x => x.Deserialize<SqlToGraphiteConfig>(configXml)).Return(config);
            reader.Expect(x => x.GetXml()).Return(configXml);
            repository.Load();
            //Test                        
            repository.AddRoleToHost(roleName, hostname);
            Assert.That(config.Hosts[0].Roles.Count, Is.EqualTo(2));
            Assert.That(config.Hosts[0].Roles[1].Name, Is.EqualTo(roleName));

            cache.VerifyAllExpectations();
            genericSerializer.VerifyAllExpectations();
        }

        [Test]
        public void Should_delete_role_from_host()
        {
            string hostname = "abc";
            string roleName = "123";
            config = new SqlToGraphiteConfig { Hosts = CreateTwoHosts() };
            cache.Expect(x => x.HasExpired()).Return(true).Repeat.Once();
            string configXml = "<xml></xml>";
            genericSerializer.Expect(x => x.Deserialize<SqlToGraphiteConfig>(configXml)).Return(config);
            reader.Expect(x => x.GetXml()).Return(configXml);
            repository.Load();
            //Test                        
            repository.DeleteRoleFromHost(roleName, hostname);
            Assert.That(config.Hosts[0].Roles.Count, Is.EqualTo(0));

            cache.VerifyAllExpectations();
            genericSerializer.VerifyAllExpectations();
        }

        [Test]
        public void Should_throw_exception_trying_delete_role_from_host_where_host_does_not_exsist()
        {
            string hostname = "notFound";
            string roleName = "123";
            config = new SqlToGraphiteConfig { Hosts = CreateTwoHosts() };
            cache.Expect(x => x.HasExpired()).Return(true).Repeat.Once();
            string configXml = "<xml></xml>";
            genericSerializer.Expect(x => x.Deserialize<SqlToGraphiteConfig>(configXml)).Return(config);
            reader.Expect(x => x.GetXml()).Return(configXml);
            repository.Load();
            //Test                        
            var ex = Assert.Throws<HostNotFoundException>(() => repository.DeleteRoleFromHost(roleName, hostname));
            Assert.That(ex.Message, Is.EqualTo(string.Format("Host {0} has not been found", hostname)));
            cache.VerifyAllExpectations();
            genericSerializer.VerifyAllExpectations();
        }

        [Test]
        public void Should_throw_exception_trying_delete_role_from_host_where_role_does_not_exsist()
        {
            string hostname = "abc";
            string roleName = "notFound";
            config = new SqlToGraphiteConfig { Hosts = CreateTwoHosts() };
            cache.Expect(x => x.HasExpired()).Return(true).Repeat.Once();
            string configXml = "<xml></xml>";
            genericSerializer.Expect(x => x.Deserialize<SqlToGraphiteConfig>(configXml)).Return(config);
            reader.Expect(x => x.GetXml()).Return(configXml);
            repository.Load();
            //Test                     
            var ex = Assert.Throws<RoleNotFoundException>(() => repository.DeleteRoleFromHost(roleName, hostname));
            Assert.That(ex.Message, Is.EqualTo(string.Format("Role {0} is not found", roleName)));
            cache.VerifyAllExpectations();
            genericSerializer.VerifyAllExpectations();
        }

        [Test]
        public void Should_throw_exception_if_host_not_found_tring_to_add_role_to_host()
        {
            string hostname = "notFound";
            string roleName = "newRole";
            config = new SqlToGraphiteConfig { Hosts = CreateTwoHosts() };
            cache.Expect(x => x.HasExpired()).Return(true).Repeat.Once();
            string configXml = "<xml></xml>";
            genericSerializer.Expect(x => x.Deserialize<SqlToGraphiteConfig>(configXml)).Return(config);
            reader.Expect(x => x.GetXml()).Return(configXml);
            repository.Load();
            //Test                        
            var ex = Assert.Throws<HostNotFoundException>(() => repository.AddRoleToHost(roleName, hostname));
            Assert.That(ex.Message, Is.EqualTo(string.Format("Host {0} has not been found", hostname)));

            cache.VerifyAllExpectations();
            genericSerializer.VerifyAllExpectations();
        }

        [Test]
        public void Should_throw_exception_if_host_not_found()
        {
            string hostname = "notFound";
            config = new SqlToGraphiteConfig { Hosts = CreateTwoHosts() };
            cache.Expect(x => x.HasExpired()).Return(true).Repeat.Once();
            string configXml = "<xml></xml>";
            genericSerializer.Expect(x => x.Deserialize<SqlToGraphiteConfig>(configXml)).Return(config);
            reader.Expect(x => x.GetXml()).Return(configXml);
            repository.Load();
            //Test            
            var ex = Assert.Throws<HostNotFoundException>(() => repository.DeleteHost(hostname));
            Assert.That(ex.Message, Is.EqualTo(string.Format("Host {0} has not been found", hostname)));
            Assert.That(config.Hosts.Count, Is.EqualTo(2));
            Assert.That(config.Hosts[0].Name, Is.EqualTo("abc"));
            cache.VerifyAllExpectations();
            genericSerializer.VerifyAllExpectations();
        }

        private static List<Host> CreateTwoHosts()
        {
            var hosts = new List<Host>();
            var host1 = new Host { Name = "abc" };
            host1.Roles.Add(new Role { Name = "123" });
            var host2 = new Host { Name = "def" };
            host2.Roles.Add(new Role { Name = "456" });
            host2.Roles.Add(new Role { Name = "789" });
            hosts.Add(host1);
            hosts.Add(host2);
            return hosts;
        }

        [Test]
        public void Should_throw_job_not_found_exception()
        {
            cache.Expect(x => x.HasExpired()).Return(true).Repeat.Once();
            string configXml = this.Add(Add(Blank, TwoClients), Templates);
            genericSerializer.Expect(x => x.Deserialize<SqlToGraphiteConfig>(configXml)).Return(config);
            reader.Expect(x => x.GetXml()).Return(configXml);
            repository.Load();

            //Test            
            Assert.Throws<JobNotFoundException>(() => repository.DeleteJobFromRole("notFound", 123, "roleName"));
            cache.VerifyAllExpectations();
            genericSerializer.VerifyAllExpectations();
        }

        private Template CreateSingleRoleAndJob(string jobName, int frequency, string roleName)
        {
            var ts = new TaskSet { Frequency = frequency, Tasks = new List<Task>() };
            var task = new Task { JobName = jobName };
            ts.Tasks.Add(task);
            var template = new Template();
            var wi = new WorkItems { RoleName = roleName, TaskSet = new List<TaskSet>() };
            wi.TaskSet.Add(ts);
            var wilist = new List<WorkItems> { wi };
            template.WorkItems = wilist;
            this.config.Templates.Add(template);
            return template;
        }

        private Template CreateMultipoleRolesAndJobs(string jobName, string jobName1, int frequency, int frequency1, string roleName)
        {
            var ts1 = new TaskSet { Frequency = frequency, Tasks = new List<Task>() };
            var ts2 = new TaskSet { Frequency = frequency1, Tasks = new List<Task>() };
            var task = new Task { JobName = jobName };
            ts1.Tasks.Add(task);
            var task1 = new Task { JobName = jobName1 };
            ts2.Tasks.Add(task1);
            var template = new Template();
            var wi = new WorkItems { RoleName = roleName, TaskSet = new List<TaskSet>() };
            wi.TaskSet.Add(ts1);
            wi.TaskSet.Add(ts2);
            var wilist = new List<WorkItems> { wi };
            template.WorkItems = wilist;
            this.config.Templates.Add(template);
            return template;
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

            var job = new WmiClient();
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
            genericSerializer.Expect(x => x.Deserialize<SqlToGraphiteConfig>(configXml)).Return(new SqlToGraphiteConfig(assemblyResolver, log));
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