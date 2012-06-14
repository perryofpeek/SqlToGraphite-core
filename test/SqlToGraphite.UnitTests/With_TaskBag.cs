using System.Collections.Generic;

using NUnit.Framework;

using Rhino.Mocks;

namespace SqlToGraphite.UnitTests
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class With_TaskBag
    {
        [Test]
        public void Should_process_a_single_task_set()
        {
            var thread = MockRepository.GenerateMock<IThread>();
            var threadList = new List<IThread> { thread };
            var taskBag = new TaskBag(threadList);

            thread.Expect(x => x.Start()); //Return IThread?
            //Test
            taskBag.Start();
            //Assert
            thread.VerifyAllExpectations();
        }

        [Test]
        public void Should_process_multiple_task_sets()
        {
            var thread1 = MockRepository.GenerateMock<IThread>();
            var thread2 = MockRepository.GenerateMock<IThread>();
            var threadList = new List<IThread> { thread1, thread2 };
            var taskBag = new TaskBag(threadList);

            thread1.Expect(x => x.Start()); //Return IThread?
            thread2.Expect(x => x.Start()); //Return IThread?
            //Test 
            taskBag.Start();
            //Assert
            thread1.VerifyAllExpectations();
        }
    }
}