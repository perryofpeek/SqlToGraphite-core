using log4net;
using NUnit.Framework;
using Rhino.Mocks;
using SqlToGraphite.Conf;

namespace SqlToGraphite.UnitTests
{
    using System.Collections.Generic;

    using SqlToGraphite.Config;

    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class With_ConfigController
    {
        private ILog log;

        private IConfigMapper configMapper;

        private IConfigRepository configRepository;

        private ConfigController configController;

        private IRoleConfigFactory roleConfigFactory;

        private IEnvironment environment;

        private ITaskSetBuilder taskSetBuilder;

        [SetUp]
        public void SetUp()
        {
            log = MockRepository.GenerateMock<ILog>();
            configMapper = MockRepository.GenerateMock<IConfigMapper>();
            configRepository = MockRepository.GenerateMock<IConfigRepository>();
            roleConfigFactory = MockRepository.GenerateMock<IRoleConfigFactory>();
            environment = MockRepository.GenerateMock<IEnvironment>();
            taskSetBuilder = MockRepository.GenerateMock<ITaskSetBuilder>();
            configController = new ConfigController(this.configMapper, this.log, this.configRepository, this.roleConfigFactory, this.environment, taskSetBuilder);
        }

        [Test]
        public void Should_return_null_object_if_validation_fails()
        {
            string path = "somePath";
            configRepository.Expect(x => x.Load());
            configRepository.Expect(x => x.Validate()).Return(false);
            //Test
            var rtn = configController.GetTaskList(path);
            //Assert
            Assert.That(rtn, Is.EqualTo(null));
            log.VerifyAllExpectations();
            configRepository.VerifyAllExpectations();
            configMapper.VerifyAllExpectations();
        }

        [Test]
        public void Should_Generate_A_Task_List()
        {
            string path = "somePath";
            var templates = new List<Template>();           

            configRepository.Expect(x => x.Load());
            configRepository.Expect(x => x.Validate()).Return(true);
            var roleConfig = MockRepository.GenerateMock<IRoleConfig>();           
            roleConfigFactory.Expect(x => x.Create(configRepository, environment)).Return(roleConfig);            
            configRepository.Expect(x => x.GetTemplates()).Return(templates);
            taskSetBuilder.Expect(x => x.BuildTaskSet(templates, roleConfig));
            //Test
            var rtn = configController.GetTaskList(path);
            //Assert
            Assert.That(rtn, Is.EqualTo(null));
            log.VerifyAllExpectations();
            configRepository.VerifyAllExpectations();
            configMapper.VerifyAllExpectations();
            roleConfigFactory.VerifyAllExpectations();
            roleConfig.VerifyAllExpectations();
            environment.VerifyAllExpectations();
        }
    }
}