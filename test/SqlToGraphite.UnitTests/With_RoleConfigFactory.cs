using NUnit.Framework;

namespace SqlToGraphite.UnitTests
{
    using Rhino.Mocks;

    using Conf;

    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class With_RoleConfigFactory
    {
        [Test]
        public void Should_return_role_config_object()
        {
            var environment = MockRepository.GenerateMock<IEnvironment>();
            var configRepository = MockRepository.GenerateMock<IConfigRepository>();
            var roleConfigFactory = new RoleConfigFactory();

            //Test
            var roleconfig = roleConfigFactory.Create(configRepository, environment);

            //Assert
            Assert.That(roleconfig, Is.Not.Null);
            Assert.That(roleconfig, Is.TypeOf<RoleConfig>());
            configRepository.VerifyAllExpectations();
        }
    }
}