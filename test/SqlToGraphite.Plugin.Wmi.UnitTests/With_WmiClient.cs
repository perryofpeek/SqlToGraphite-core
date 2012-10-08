using log4net;

using NUnit.Framework;

using Rhino.Mocks;

using SqlToGraphiteInterfaces;

namespace SqlToGraphite.Plugin.Wmi.UnitTests
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class With_WmiClient
    {
        [Test, Ignore()]
        public void Should_get_wmi_data()
        {
            Job taskParams = null;
            var log = MockRepository.GenerateMock<ILog>();
            //var taskParams = new TaskParams("path", "SELECT PercentFreeSpace, Name FROM Win32_PerfFormattedData_PerfDisk_LogicalDisk Where Name <> '_Total'", string.Empty, "wmi", "nme", "statsdudp");
            var wmiClient = new WmiClient(log, taskParams);
            var results = wmiClient.Get();
            Assert.That(results[0].Value, Is.GreaterThan(0));
            Assert.That(results[0].Name == "C" || results[0].Name == "D");
            Assert.That(results[1].Value, Is.GreaterThan(0));
            Assert.That(results[1].Name == "C" || results[1].Name == "D");
        }

        //[Test]
        //public void Should_get_wmi_data_using_connection_string()
        //{
        //    const string ConnectionString = "Username=notFound;Password=abcd1234;";
        //    var log = MockRepository.GenerateMock<ILog>();
        //    var taskParams = new TaskParams("path", "SELECT PercentFreeSpace, Name FROM Win32_PerfFormattedData_PerfDisk_LogicalDisk Where Name <> '_Total'", ConnectionString, "wmi", "nme", "statsdudp");
        //    var wmiClient = new WmiClient(log, taskParams);
        //    var results = wmiClient.Get();
        //    Assert.That(results[0].Value, Is.GreaterThan(0));
        //    Assert.That(results[0].Name == "C" || results[0].Name == "D");
        //    Assert.That(results[1].Value, Is.GreaterThan(0));
        //    Assert.That(results[1].Name == "C" || results[1].Name == "D");
        //}
    }
}