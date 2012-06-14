using NUnit.Framework;

using SqlToGraphite.Clients;

namespace SqlToGraphite.UnitTests
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class With_KnownDataClients
    {
        [Test]
        public void Should_know_sqlserver()
        {
            var knownGraphiteClients = new KnownDataClients();
            Assert.IsTrue(knownGraphiteClients.IsKnown("sqlserver"));
        }

        [Test]
        public void Should_not_know_other()
        {
            var knownGraphiteClients = new KnownDataClients();
            Assert.IsFalse(knownGraphiteClients.IsKnown("unknown"));
        }

        [Test]
        public void Should_know_oracle()
        {
            var knownGraphiteClients = new KnownDataClients();
            Assert.IsTrue(knownGraphiteClients.IsKnown("oracle"));
        }

        [Test]
        public void Should_know_wmi()
        {
            var knownGraphiteClients = new KnownDataClients();
            Assert.IsTrue(knownGraphiteClients.IsKnown("wmi"));
        }
    }
}