using NUnit.Framework;

using SqlToGraphite.Clients;

namespace SqlToGraphite.UnitTests
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class With_KnownGraphiteClients
    {
        [Test]
        public void Should_know_statsdudp()
        {
            var knownGraphiteClients = new KnownGraphiteClients();
            Assert.IsTrue(knownGraphiteClients.IsKnown("statsdudp"));
        }

        [Test]
        public void Should_not_know_other()
        {
            var knownGraphiteClients = new KnownGraphiteClients();
            Assert.IsFalse(knownGraphiteClients.IsKnown("unknown"));
        }

        [Test]
        public void Should_know_nativetcp()
        {
            var knownGraphiteClients = new KnownGraphiteClients();
            Assert.IsTrue(knownGraphiteClients.IsKnown("graphitetcp"));
        }
    }
}