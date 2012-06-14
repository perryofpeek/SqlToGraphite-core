using log4net;
using NUnit.Framework;
using Rhino.Mocks;

namespace SqlToGraphite.UnitTests
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class With_Controller
    {
        private ILog logMock;

        private ISleep sleep;

        private IStop stop;

        private ITaskSet taskSet;

        private Controller controller;

        [SetUp]
        public void SetUp()
        {
            logMock = MockRepository.GenerateMock<ILog>();
            sleep = MockRepository.GenerateMock<ISleep>();
            stop = MockRepository.GenerateMock<IStop>();
            taskSet = MockRepository.GenerateMock<ITaskSet>();
        }

        [Test]
        public void Should_run_task_set()
        {
            var length = 500;
            taskSet.Expect(x => x.Frequency).Return(length);
            controller = new Controller(taskSet, sleep, stop, logMock);
            stop.Expect(x => x.ShouldStop()).Return(false).Repeat.Once();
            stop.Expect(x => x.ShouldStop()).Return(true).Repeat.Once();            
            sleep.Expect(x => x.SleepSeconds(length));
            taskSet.Expect(x => x.Process());
            //Test
            controller.Process();

            //Assert
            sleep.VerifyAllExpectations();
            stop.VerifyAllExpectations();
            taskSet.VerifyAllExpectations();
        }
    }
}