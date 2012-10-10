using System;
using System.Collections.Generic;
using log4net;
using NUnit.Framework;
using Rhino.Mocks;
using SqlToGraphite.Conf;
using SqlToGraphite.Config;
using SqlToGraphite.Plugin.SqlServer;

// ReSharper disable InconsistentNaming
namespace SqlToGraphite.UnitTests
{
    [TestFixture]
    public class With_ConfigMapper
    {
        private IDataClientFactory dataClientFactory;

        private IGraphiteClientFactory graphiteClientFactory;

        private IConfigRepository configRepository;

        private IStop stop;

        private ILog log;

        [SetUp]
        public void SetUp()
        {
            dataClientFactory = MockRepository.GenerateMock<IDataClientFactory>();
            graphiteClientFactory = MockRepository.GenerateMock<IGraphiteClientFactory>();
            stop = MockRepository.GenerateMock<IStop>();
            log = MockRepository.GenerateMock<ILog>();
            configRepository = MockRepository.GenerateMock<IConfigRepository>();
        }

        [Test]
        public void Should_throw_exception_if_graphite_client_is_unknown()
        {
            string clientName = "notKnown";
            string jobName = "someJob";
            var job = new SqlServerClient();
            var msg = "some exception message";
            job.ClientName = clientName;
            job.Name = jobName;
            configRepository.Expect(x => x.GetClient(clientName)).Throw(new ClientNotFoundException(msg));
            configRepository.Expect(x => x.GetJob(jobName)).Return(job);
            string hostname = "hostname";
            var configMapper = new ConfigMapper(hostname, this.stop, this.dataClientFactory, this.graphiteClientFactory, this.log, configRepository);
            var taskSets = new List<TaskSet>();
            var taskSet = new TaskSet { Frequency = 100, Tasks = new List<Task>() };
            var t = new Task { JobName = jobName };

            taskSet.Tasks.Add(t);

            taskSets.Add(taskSet);
            //Test
            var ex = Assert.Throws<ClientNotFoundException>(() => configMapper.Map(taskSets));
            //Assert
            Assert.That(ex.Message, Is.EqualTo(msg));
        }

        [Test]
        public void Should_map_sucessfully()
        {
            string hostname = "hostname";
            var clientName = "someClient";
            var freq = 100;
            var configMapper = new ConfigMapper(hostname, this.stop, this.dataClientFactory, this.graphiteClientFactory, this.log, configRepository);
            var taskSets = new List<TaskSet>();
            var taskSet = new TaskSet { Frequency = freq, Tasks = new List<Task>() };
            string name = new Guid().ToString();
            Job job = new SqlServerClient();
            job.ClientName = clientName;

            configRepository.Expect(y => y.GetJob(name)).Return(job);
            configRepository.Expect(y => y.GetClient(clientName)).Return(new GraphiteTcpClient());

            taskSet.Tasks.Add(new Task() { JobName = name });
            taskSets.Add(taskSet);
            //Test
            var taskList = configMapper.Map(taskSets);
            //Assert
            Assert.That(taskList.Count, Is.EqualTo(1));
            Assert.That(taskList[0].Frequency, Is.EqualTo(freq));
            Assert.That(taskList[0].Tasks.Count, Is.EqualTo(1));
            Assert.That(taskList[0].Tasks[0].GetType(), Is.EqualTo(typeof(RunableRunTask)));
            configRepository.VerifyAllExpectations();
        }
    }
}
