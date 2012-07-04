using System.Collections.Generic;
using log4net;
using NUnit.Framework;
using Rhino.Mocks;
using SqlToGraphiteInterfaces;

namespace SqlToGraphite.UnitTests
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class With_task
    {
        private IDataClientFactory dataClientFactory;

        private ISqlClient sqlClient;

        private IStatsClient statsClient;

        private IGraphiteClientFactory graphiteClientFactory;

        private ILog log;

        [SetUp]
        public void SetUp()
        {
            this.dataClientFactory = MockRepository.GenerateMock<IDataClientFactory>();
            this.sqlClient = MockRepository.GenerateMock<ISqlClient>();
            this.graphiteClientFactory = MockRepository.GenerateMock<IGraphiteClientFactory>();
            statsClient = MockRepository.GenerateMock<IStatsClient>();
            log = MockRepository.GenerateMock<ILog>();
        }

        [Test]
        public void Should_run_task_sending_one_result()
        {
            var result = MockRepository.GenerateMock<IResult>();
            var resultList = new List<IResult> { result };
            var param = new TaskParams("path", "sql", "cs", "SqlServer", "name", "client");
            this.dataClientFactory.Expect(x => x.Create(param)).Return(this.sqlClient);
            this.sqlClient.Expect(x => x.Get()).Return(resultList);
            statsClient.Expect(x => x.Send(result));
            var graphiteParams = new GraphiteParams("host", 1234);
            this.graphiteClientFactory.Expect(x => x.Create(graphiteParams, param)).Return(this.statsClient);
            ITask task = new Task(param, this.dataClientFactory, this.graphiteClientFactory, graphiteParams, log);
            //Test
            task.Process();
            //Assert
            this.sqlClient.VerifyAllExpectations();
            this.dataClientFactory.VerifyAllExpectations();
            this.graphiteClientFactory.VerifyAllExpectations();
            statsClient.VerifyAllExpectations();
        }

        [Test]
        public void Should_run_task_sending_two_results()
        {
            var result1 = MockRepository.GenerateMock<IResult>();
            var result2 = MockRepository.GenerateMock<IResult>();
            var resultList = new List<IResult> { result1, result2 };
            var param = new TaskParams("path", "sql", "cs", "SqlServer", "name", "client");
            var graphiteParams = new GraphiteParams("host", 1234);
            this.dataClientFactory.Expect(x => x.Create(param)).Return(this.sqlClient);
            this.graphiteClientFactory.Expect(x => x.Create(graphiteParams, param)).Return(this.statsClient);
            this.sqlClient.Expect(x => x.Get()).Return(resultList);
            statsClient.Expect(x => x.Send(result1)).Repeat.Once();
            statsClient.Expect(x => x.Send(result2)).Repeat.Once();
            ITask task = new Task(param, this.dataClientFactory, this.graphiteClientFactory, graphiteParams, log);
            //Test
            task.Process();
            //Assert
            this.sqlClient.VerifyAllExpectations();
            this.dataClientFactory.VerifyAllExpectations();
            this.graphiteClientFactory.VerifyAllExpectations();
            statsClient.VerifyAllExpectations();
        }
    }
}