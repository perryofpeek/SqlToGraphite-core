using System.IO;

using NUnit.Framework;

using SqlToGraphite.Clients;

namespace SqlToGraphite.UnitTests
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class With_config
    {
        //[Test]
        //public void Should_read_configuration_client_list()
        //{
        //    var config = new Config(new StreamReader("TestConfigSchema.xml"), new KnownGraphiteClients());
        //    var clientList = config.GetClientList();
        //    Assert.That(clientList.Count, Is.EqualTo(2));
        //    Assert.That(clientList["statsdUdp"].name, Is.EqualTo("statsdUdp"));
        //    Assert.That(clientList["statsdUdp"].port, Is.EqualTo("1234"));
        //}

        //[Test]
        //public void Should_read_configuration_and_throw_UnknownGraphiteClientTypeException()
        //{
        //    var xml = "<SqlToGraphiteConfig><Clients><Client name='unknown' port='1234' /></Clients></SqlToGraphiteConfig>";
        //    var sr = Helper.GetStreamReader(xml);
        //    var ex = Assert.Throws<UnknownGraphiteClientTypeException>(() => new Config(sr, new KnownGraphiteClients()));
        //    Assert.That(ex.Message, Is.EqualTo("unknown is unknown"));
        //    sr.Close();
        //}

        //[Test]
        //public void Should_read_configuration_task_sets()
        //{
        //    var config = new Config(new StreamReader("TestConfigSchema.xml"), new KnownGraphiteClients());
        //    var taskSets = config.GetWorkItems();
        //    Assert.That(taskSets.Count, Is.EqualTo(2));
            //Assert.That(taskSets[0].frequency, Is.EqualTo("1000"));
            //Assert.That(taskSets[0].Task[0].client, Is.EqualTo("client"));
            //Assert.That(taskSets[0].Task[0].connectionstring, Is.EqualTo("cs"));
            //Assert.That(taskSets[0].Task[0].name, Is.EqualTo("name"));
            //Assert.That(taskSets[0].Task[0].path, Is.EqualTo("some.path"));
            //Assert.That(taskSets[0].Task[0].port, Is.EqualTo("12345"));
            //Assert.That(taskSets[0].Task[0].sql, Is.EqualTo("sql"));
            //Assert.That(taskSets[0].Task[0].type, Is.EqualTo("type"));
        //}
    }
}