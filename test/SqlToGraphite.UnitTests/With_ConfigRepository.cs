using System;
using log4net;
using NUnit.Framework;
using Rhino.Mocks;
using SqlToGraphite.Clients;
using SqlToGraphite.Conf;

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

        private int sleepTime;
        [SetUp]
        public void SetUp()
        {
            sleepTime = 1000;
            reader = MockRepository.GenerateMock<IConfigReader>();
            cache = MockRepository.GenerateMock<ICache>();
            sleep = MockRepository.GenerateMock<ISleep>();
            log = MockRepository.GenerateMock<ILog>();
            repository = new ConfigRepository(this.reader, new KnownGraphiteClients(), this.cache, this.sleep, this.log, sleepTime);
        }

        private string Add(string input, string data)
        {
            return input.Replace(End, string.Format("{0}{1}", data, End));
        }

        [Test]
        public void Should_read_config_and_get_clients()
        {
            var configXml = this.Add(Blank, TwoClients);
            reader.Expect(x => x.GetXml()).Return(configXml);

            cache.Expect(x => x.HasExpired()).Return(true).Repeat.Once();
            cache.Expect(x => x.ResetCache()).Repeat.Once();
            repository.Load();
            Assert.That(repository.GetClients().Count, Is.EqualTo(2));
            reader.VerifyAllExpectations();
            cache.VerifyAllExpectations();
        }

        [Test]
        public void Should_retry_to_read_config_on_getting_error()
        {
            var configXml = this.Add(Blank, TwoClients);
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
        }

        [Test]
        public void Should_load_config_and_get_clients()
        {
            string configXml = this.Add(Blank, TwoClients);
            reader.Expect(x => x.GetXml()).Return(configXml);
            cache.Expect(x => x.HasExpired()).Return(true).Repeat.Once();
            repository.Load();
            Assert.That(repository.GetClients().Count, Is.EqualTo(2));
            reader.VerifyAllExpectations();
            cache.VerifyAllExpectations();
        }

        [Test]
        public void Should_read_config_and_get_clients_dictionary()
        {
            string configXml = this.Add(Blank, TwoClients);
            reader.Expect(x => x.GetXml()).Return(configXml);
            cache.Expect(x => x.HasExpired()).Return(true).Repeat.Once();
            repository.Load();
            Assert.That(repository.GetClients().Count, Is.EqualTo(2));
            Assert.That(repository.GetClientList().Get("statsdudp").Name.ToLower(), Is.EqualTo("statsdudp"));
            Assert.That(repository.GetClientList().Get("graphitetcp").Name.ToLower(), Is.EqualTo("graphitetcp"));
            reader.VerifyAllExpectations();
            cache.VerifyAllExpectations();
        }

        [Test]
        public void Should_read_config_and_get_error_with_clients_dictionary()
        {
            string configXml = this.Add(Blank, UnknownClient);
            reader.Expect(x => x.GetXml()).Return(configXml);
            cache.Expect(x => x.HasExpired()).Return(true).Repeat.Once();
            repository.Load();
            Assert.That(repository.Errors.Count, Is.EqualTo(1));
            Assert.That(repository.Errors[0].Contains(ConfigRepository.UnknownClient), Is.EqualTo(true));
            Assert.That(repository.Validate(), Is.EqualTo(false));
            reader.VerifyAllExpectations();
            cache.VerifyAllExpectations();
        }

        [Test]
        public void Should_read_config_and_get_Templates()
        {
            string configXml = this.Add(Blank, Templates);
            reader.Expect(x => x.GetXml()).Return(configXml);
            cache.Expect(x => x.HasExpired()).Return(true).Repeat.Once();
            repository.Load();
            Assert.That(repository.GetTemplates().Count, Is.EqualTo(2));
            reader.VerifyAllExpectations();
            cache.VerifyAllExpectations();
        }

        [Test]
        public void Should_read_config_from_cache()
        {
            string configXml = this.Add(Blank, Templates);
            reader.Expect(x => x.GetXml()).Return(configXml).Repeat.Once();
            cache.Expect(x => x.HasExpired()).Return(true).Repeat.Once();
            cache.Expect(x => x.HasExpired()).Return(false).Repeat.Once();
            repository.Load();
            repository.Load();
            Assert.That(repository.GetTemplates().Count, Is.EqualTo(2));
            reader.VerifyAllExpectations();
            cache.VerifyAllExpectations();
        }

        [Test]
        public void Should_read_config_and_get_Hosts()
        {
            string configXml = this.Add(Blank, TwoHosts);
            reader.Expect(x => x.GetXml()).Return(configXml);
            cache.Expect(x => x.HasExpired()).Return(true).Repeat.Once();
            repository.Load();
            Assert.That(repository.GetHosts().Count, Is.EqualTo(4));
            reader.VerifyAllExpectations();
            cache.VerifyAllExpectations();
        }

        [Test]
        public void Should_not_validate_because_templates_are_not_defined()
        {
            string configXml = this.Add(Add(Blank, TwoClients), TwoHosts);
            reader.Expect(x => x.GetXml()).Return(configXml);
            cache.Expect(x => x.HasExpired()).Return(true).Repeat.Once();
            repository.Load();
            Assert.That(repository.Validate(), Is.EqualTo(false));
            Assert.That(repository.Errors.Contains(ConfigRepository.FailedToFindTemplates), Is.EqualTo(true));
            reader.VerifyAllExpectations();
            cache.VerifyAllExpectations();
        }

        [Test]
        public void Should_not_validate_because_hosts_are_not_defined()
        {
            string configXml = this.Add(Add(Blank, TwoClients), Templates);
            reader.Expect(x => x.GetXml()).Return(configXml);
            cache.Expect(x => x.HasExpired()).Return(true).Repeat.Once();
            repository.Load();
            Assert.That(repository.Validate(), Is.EqualTo(false));
            Assert.That(repository.Errors.Contains(ConfigRepository.FailedToFindHosts), Is.EqualTo(true));
            reader.VerifyAllExpectations();
            cache.VerifyAllExpectations();
        }

        [Test]
        public void Should_not_validate_because_clients_are_not_defined()
        {
            string configXml = this.Add(Add(Blank, TwoHosts), Templates);
            reader.Expect(x => x.GetXml()).Return(configXml);
            cache.Expect(x => x.HasExpired()).Return(true).Repeat.Once();
            repository.Load();
            Assert.That(repository.Validate(), Is.EqualTo(false));
            Assert.That(repository.Errors.Contains(ConfigRepository.FailedToFindClients), Is.EqualTo(true));
            reader.VerifyAllExpectations();
            cache.VerifyAllExpectations();
        }

        [Test]
        public void Should_validate_sucessfully()
        {
            string configXml = this.Add(Add(Add(Blank, TwoHosts), Templates), TwoClients);
            reader.Expect(x => x.GetXml()).Return(configXml);
            cache.Expect(x => x.HasExpired()).Return(true).Repeat.Once();
            repository.Load();
            Assert.That(repository.Validate(), Is.EqualTo(true));
            Assert.That(repository.Errors.Count, Is.EqualTo(0));
            reader.VerifyAllExpectations();
            cache.VerifyAllExpectations();
        }

        [Test]
        public void Should_return_failed_to_load_config_error()
        {
            var configXml = this.Add(Blank, TwoClients);
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
        }
    }
}