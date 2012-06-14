using System.Collections.Generic;
using log4net;
using NUnit.Framework;
using Rhino.Mocks;
using SqlToGraphite.Conf;

namespace SqlToGraphite.UnitTests
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class With_TaskManager
    {
        private ILog log;

        private IConfigController configController;

        private string path;

        private IStop stop;

        private ISleep sleeper;

        [SetUp]
        public void SetUp()
        {
            log = MockRepository.GenerateMock<ILog>();
            stop = MockRepository.GenerateMock<IStop>();
            sleeper = MockRepository.GenerateMock<ISleep>();
            configController = MockRepository.GenerateMock<IConfigController>();
            path = "somePath";
        }

        [Test]
        public void Should_start_all_controller()
        {
            int time = 1000;
            var taskBag = MockRepository.GenerateMock<ITaskBag>();
            configController.Expect(x => x.GetTaskBag(path)).Return(taskBag);
            var taskManager = new TaskManager(log, configController, path, stop, sleeper, time);
            stop.Expect(x => x.ShouldStop()).Return(false).Repeat.Once();
            stop.Expect(x => x.ShouldStop()).Return(true).Repeat.Once();
            sleeper.Expect(x => x.Sleep(time));
            taskBag.Expect(x => x.Start());
            //Test
            taskManager.Start();
            //Assert
            taskBag.VerifyAllExpectations();
            stop.VerifyAllExpectations();
            sleeper.VerifyAllExpectations();
        }

        [Test]
        public void Should_stop_all_controllers()
        {
            var taskBag = MockRepository.GenerateMock<ITaskBag>();            
            var taskManager = new TaskManager(log, configController, path, stop, sleeper, 1);
            taskManager.TaskBag = taskBag;
            taskBag.Expect(x => x.Stop());
            //Test
            taskManager.Stop();
            //Assert
            taskBag.VerifyAllExpectations();
        }
    }
}