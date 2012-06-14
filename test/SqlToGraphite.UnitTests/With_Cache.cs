using System;
using System.Threading;
using log4net;
using NUnit.Framework;
using Rhino.Mocks;
using SqlToGraphite.Conf;

namespace SqlToGraphite.UnitTests
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class With_Cache
    {
        [Test]
        public void Should_expire_after_One_second()
        {
            var log = MockRepository.GenerateMock<ILog>();
            var t = new TimeSpan(0, 0, 0, 0, 500);
            var cache = new Cache(t, log);
            cache.ResetCache();
            Assert.That(cache.HasExpired(), Is.EqualTo(false));
            Thread.Sleep(1000);
            Assert.That(cache.HasExpired(), Is.EqualTo(true));
        }

        [Test]
        public void Should_expire_after_construction()
        {
            var log = MockRepository.GenerateMock<ILog>();
            var t = new TimeSpan(0, 0, 0, 0, 500);
            var cache = new Cache(t, log);
            Assert.That(cache.HasExpired(), Is.EqualTo(true));
        }
    }
}