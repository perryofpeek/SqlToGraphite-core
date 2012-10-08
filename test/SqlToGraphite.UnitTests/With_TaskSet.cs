using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;

namespace SqlToGraphite.UnitTests
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class With_TaskSet
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void Should_Process_single_task()
        {
            int freq = 1;
            var task = MockRepository.GenerateMock<IRunTask>();
            task.Expect(x => x.Process());
            var tasks = new List<IRunTask> { task };
            IStop stop = MockRepository.GenerateMock<IStop>();
            stop.Expect(x => x.ShouldStop()).Return(false).Repeat.Once();
            stop.Expect(x => x.ShouldStop()).Return(true).Repeat.Once();
            var taskSet = new RunTaskSetWithProcess(tasks, stop, freq);

            taskSet.Process();

            task.VerifyAllExpectations();
            stop.VerifyAllExpectations();
        }

        [Test]
        public void Should_Process_multiple_tasks()
        {
            int freq = 2;
            var task1 = MockRepository.GenerateMock<IRunTask>();
            task1.Expect(x => x.Process()).Repeat.Once();
            var task2 = MockRepository.GenerateMock<IRunTask>();
            task2.Expect(x => x.Process()).Repeat.Once();
            var tasks = new List<IRunTask> { task1, task2 };
            IStop stop = MockRepository.GenerateMock<IStop>();
            stop.Expect(x => x.ShouldStop()).Return(false).Repeat.Once();
            stop.Expect(x => x.ShouldStop()).Return(true).Repeat.Once();
            var taskSet = new RunTaskSetWithProcess(tasks, stop, freq);

            taskSet.Process();

            task1.VerifyAllExpectations();
            task2.VerifyAllExpectations();
            stop.VerifyAllExpectations();
        }

        [Test]
        public void Should_Stop_after_Processing_multiple_tasks()
        {
            int freq = 2;
            var task1 = MockRepository.GenerateMock<IRunTask>();
            task1.Expect(x => x.Process()).Repeat.Once();
            var task2 = MockRepository.GenerateMock<IRunTask>();
            task2.Expect(x => x.Process()).Repeat.Once();
            var tasks = new List<IRunTask> { task1, task2 };
            var stop = MockRepository.GenerateMock<IStop>();
            stop.Expect(x => x.ShouldStop()).Return(false).Repeat.Once();
            stop.Expect(x => x.ShouldStop()).Return(true).Repeat.Once();
            var taskSet = new RunTaskSetWithProcess(tasks, stop, freq);

            taskSet.Process();

            task1.VerifyAllExpectations();
            task2.VerifyAllExpectations();
            stop.VerifyAllExpectations();
        }
    }
}