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

        private IRunTaskSet runTaskSet;

        private Controller controller;

        [SetUp]
        public void SetUp()
        {
            logMock = MockRepository.GenerateMock<ILog>();
            sleep = MockRepository.GenerateMock<ISleep>();
            stop = MockRepository.GenerateMock<IStop>();
            this.runTaskSet = MockRepository.GenerateMock<IRunTaskSet>();
        }

        [Test]
        public void Should_run_task_set()
        {
            var length = 500;
            this.runTaskSet.Expect(x => x.Frequency).Return(length);
            controller = new Controller(this.runTaskSet, sleep, stop, logMock);
            stop.Expect(x => x.ShouldStop()).Return(false).Repeat.Once();
            stop.Expect(x => x.ShouldStop()).Return(true).Repeat.Once();            
            sleep.Expect(x => x.SleepSeconds(length));
            this.runTaskSet.Expect(x => x.Process());
            //Test
            controller.Process();

            //Assert
            sleep.VerifyAllExpectations();
            stop.VerifyAllExpectations();
            this.runTaskSet.VerifyAllExpectations();
        }
    }
}