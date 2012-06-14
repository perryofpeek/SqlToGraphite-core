using NUnit.Framework;

namespace SqlToGraphite.UnitTests
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class With_GraphiteClients
    {
        [Test]
        public void Should_Add_client()
        {
            var name = "name";
            var graphiteClients = new GraphiteClients();
            graphiteClients.Add(name, "1234");
            var client = graphiteClients.Get(name);
            Assert.That(client.Name, Is.EqualTo(name));
            Assert.That(client.Port, Is.EqualTo(1234));
        }

        [Test]
        public void Should_throw_ClientNotFoundException_as_client_is_not_known()
        {
            var name = "name";
            var graphiteClients = new GraphiteClients();
            graphiteClients.Add(name, "1234");
            var ex = Assert.Throws<ClientNotFoundException>(() => graphiteClients.Get("notFound"));
            Assert.That(ex.Message, Is.EqualTo("Client notFound is not known add this into the conifiguration xml"));
        }

        [Test]
        public void Should_not_care_about_client_name_case()
        {
            var name = "name";
            var graphiteClients = new GraphiteClients();
            graphiteClients.Add(name, "1234");
            var client = graphiteClients.Get("NaMe");
            Assert.That(client.Name, Is.EqualTo(name));
            Assert.That(client.Port, Is.EqualTo(1234));
        }
    }
}