using System;
using System.IO;

using NUnit.Framework;

using SqlToGraphite.Conf;

namespace SqlToGraphite.UnitTests
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class With_ConfigWriter
    {
        [Test]
        public void Should_Store()
        {
            var fileName = Guid.NewGuid().ToString();
            var data = "some data";
            IConfigWriter configWriter = new ConfigFileWriter(fileName);
            configWriter.Save(data);
            var fileData = File.ReadAllText(fileName);
            Assert.That(data, Is.EqualTo(fileData));
        }
    }
}