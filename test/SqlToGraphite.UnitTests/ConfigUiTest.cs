using NUnit.Framework;

using SqlToGraphite.Conf;

namespace SqlToGraphite.UnitTests
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class ConfigUiTest
    {
        [Test, Ignore()]
        public void Should_Add_thing()
        {
            var configRepository = new ConfigRepository(null, null, null, null, null, 5);
            Assert.IsFalse(true);
        }
    }
}