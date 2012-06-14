using System;
using System.Collections.Generic;
using log4net;
using NUnit.Framework;
using Rhino.Mocks;

// ReSharper disable InconsistentNaming
namespace SqlToGraphite.UnitTests
{
    [TestFixture]
    public class With_ConfigMapper
    {
        private IDataClientFactory dataClientFactory;

        private IGraphiteClientFactory graphiteClientFactory;

        private IStop stop;

        private ILog log;

        [SetUp]
        public void SetUp()
        {
            dataClientFactory = MockRepository.GenerateMock<IDataClientFactory>();
            graphiteClientFactory = MockRepository.GenerateMock<IGraphiteClientFactory>();
            stop = MockRepository.GenerateMock<IStop>();
            log = MockRepository.GenerateMock<ILog>();
        }

        [Test]
        public void Should_throw_exception_if_graphite_client_is_unknown()
        {
            string hostname = "hostname";
            var configMapper = new ConfigMapper(hostname, stop, dataClientFactory, graphiteClientFactory, log);
            var taskSet = new List<SqlToGraphiteConfigTemplatesWorkItemsTaskSet>();
            var x = new SqlToGraphiteConfigTemplatesWorkItemsTaskSet { frequency = "100", Task = new SqlToGraphiteConfigTemplatesWorkItemsTaskSetTask[1] };
            x.Task[0] = new SqlToGraphiteConfigTemplatesWorkItemsTaskSetTask
                {
                    client = "client",
                    connectionstring = "cs",
                    name = "name",
                    path = "path",
                    port = "1234",
                    sql = "sql",
                    type = "typr"
                };
            taskSet.Add(x);
            var clients = new GraphiteClients();
            var ex = Assert.Throws<ClientNotFoundException>(() => configMapper.Map(taskSet, clients));
            Assert.That(ex.Message, Is.EqualTo("Client client is not known add this into the conifiguration xml"));
        }

        [Test]
        public void Should_map_sucessfully()
        {
            string hostname = "hostname";
            var client = "someClient";
            var freq = 100;
            var configMapper = new ConfigMapper(hostname, stop, dataClientFactory, graphiteClientFactory, log);
            var taskSet = new List<SqlToGraphiteConfigTemplatesWorkItemsTaskSet>();
            var x = new SqlToGraphiteConfigTemplatesWorkItemsTaskSet { frequency = freq.ToString(), Task = new SqlToGraphiteConfigTemplatesWorkItemsTaskSetTask[1] };
            x.Task[0] = new SqlToGraphiteConfigTemplatesWorkItemsTaskSetTask
            {
                client = client,
                connectionstring = "cs",
                name = "name",
                path = "path",
                port = "1234",
                sql = "sql",
                type = "type"
            };
            taskSet.Add(x);
            var clients = new GraphiteClients();
            clients.Add(client, "1234");
            var taskList  = configMapper.Map(taskSet, clients);
            Assert.That(taskList.Count, Is.EqualTo(1));
            Assert.That(taskList[0].Frequency, Is.EqualTo(freq));
            Assert.That(taskList[0].Tasks.Count, Is.EqualTo(1));
            Assert.That(taskList[0].Tasks[0].GetType(), Is.EqualTo(typeof(Task)));
        }       
    }
}
