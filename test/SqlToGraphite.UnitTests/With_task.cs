using System.Collections.Generic;
using log4net;
using NUnit.Framework;
using Rhino.Mocks;
using SqlToGraphite.Plugin.Wmi;
using SqlToGraphiteInterfaces;

namespace SqlToGraphite.UnitTests
{
    using System;

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
            var param = new WmiClient();
            var client = new GraphiteTcpClient();
            this.dataClientFactory.Expect(x => x.Create(param)).Return(this.sqlClient);
            this.sqlClient.Expect(x => x.Get()).Return(resultList);
            statsClient.Expect(x => x.Send(resultList));
            this.graphiteClientFactory.Expect(x => x.Create(client)).Return(this.statsClient);
            IRunTask runTask = new RunableRunTask(param, this.dataClientFactory, this.graphiteClientFactory, this.log, client);
            //Test
            runTask.Process();
            //Assert
            this.sqlClient.VerifyAllExpectations();
            this.dataClientFactory.VerifyAllExpectations();
            this.graphiteClientFactory.VerifyAllExpectations();
            statsClient.VerifyAllExpectations();
        }

        [Test]
        public void Should_run_task_sending_one_result_not_fail_on_exception()
        {
            var result = MockRepository.GenerateMock<IResult>();
            var resultList = new List<IResult> { result };            
            var param = new WmiClient();
            var client = new GraphiteTcpClient();
            this.dataClientFactory.Expect(x => x.Create(param)).Return(this.sqlClient);
            this.sqlClient.Expect(x => x.Get()).Return(resultList);
            statsClient.Expect(x => x.Send(resultList)).Throw(new ApplicationException());
            this.graphiteClientFactory.Expect(x => x.Create(client)).Return(this.statsClient);
            IRunTask runTask = new RunableRunTask(param, this.dataClientFactory, this.graphiteClientFactory, this.log, client);
            //Test
            runTask.Process();
            //Assert
            this.sqlClient.VerifyAllExpectations();
            this.dataClientFactory.VerifyAllExpectations();
            this.graphiteClientFactory.VerifyAllExpectations();
            statsClient.VerifyAllExpectations();
        }

        [Test]
        public void Should_run_task_sending_two_results_exception_on_sending_the_first()
        {
            var job = new WmiClient();
            var client = new GraphiteTcpClient();
            var result1 = MockRepository.GenerateMock<IResult>();
            var result2 = MockRepository.GenerateMock<IResult>();
            var resultList = new List<IResult> { result1, result2 };
            this.dataClientFactory.Expect(x => x.Create(job)).Return(this.sqlClient);
            this.graphiteClientFactory.Expect(x => x.Create(client)).Return(this.statsClient);
            this.sqlClient.Expect(x => x.Get()).Return(resultList);
            statsClient.Expect(x => x.Send(resultList)).Throw(new ApplicationException());            
            IRunTask runTask = new RunableRunTask(job, this.dataClientFactory, this.graphiteClientFactory, this.log, client);
            //Test
            runTask.Process();
            //Assert
            this.sqlClient.VerifyAllExpectations();
            this.dataClientFactory.VerifyAllExpectations();
            this.graphiteClientFactory.VerifyAllExpectations();
            statsClient.VerifyAllExpectations();
        }

        [Test]
        public void Should_run_task_sending_two_results()
        {
            var job = new WmiClient();
            var client = new GraphiteTcpClient();
            var result1 = MockRepository.GenerateMock<IResult>();
            var result2 = MockRepository.GenerateMock<IResult>();
            var resultList = new List<IResult> { result1, result2 };            
            this.dataClientFactory.Expect(x => x.Create(job)).Return(this.sqlClient);
            this.graphiteClientFactory.Expect(x => x.Create(client)).Return(this.statsClient);
            this.sqlClient.Expect(x => x.Get()).Return(resultList);
            statsClient.Expect(x => x.Send(resultList)).Repeat.Once();            
            IRunTask runTask = new RunableRunTask(job, this.dataClientFactory, this.graphiteClientFactory, this.log, client);
            //Test
            runTask.Process();
            //Assert
            this.sqlClient.VerifyAllExpectations();
            this.dataClientFactory.VerifyAllExpectations();
            this.graphiteClientFactory.VerifyAllExpectations();
            statsClient.VerifyAllExpectations();
        }
    }
}