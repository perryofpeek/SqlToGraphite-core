using System;
using System.IO;
using System.Net;

using NUnit.Framework;

namespace SqlToGraphite.UnitTests
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class With_ConfigReader
    {
        private ConfigReader configRepository;

        private string diskPath;

        [SetUp]
        public void SetUp()
        {
            var filename = Guid.NewGuid().ToString();
            diskPath = @"c:\" + filename;
            var uriPath = "file:///C:/" + filename;
            configRepository = new ConfigReader(uriPath, string.Empty, string.Empty);
        }

        [Test]
        public void Should_get_config()
        {
            File.WriteAllText(diskPath, "test123");
            var s = configRepository.GetXml();
            Assert.That(s, Is.EqualTo("test123"));
            File.Delete(diskPath);
        }

        [Test]
        public void Should_get_config_as_Xml_Doc()
        {
            File.WriteAllText(diskPath, "<root />");
            var xmlDocument = configRepository.GetXml();
            Assert.That(xmlDocument, Is.EqualTo("<root />"));
            File.Delete(diskPath);
        }

        [Test]
        public void Should_return_true_if_config_has_changed()
        {
            File.WriteAllText(diskPath, "<root />");
            var xmlDocument = configRepository.GetXml();
            Assert.That(configRepository.HasChanged(), Is.EqualTo(true));
            Assert.That(xmlDocument, Is.EqualTo("<root />"));
            File.Delete(diskPath);
        }

        [Test]
        public void Should_return_false_if_config_has_changed()
        {
            File.WriteAllText(diskPath, "<root />");
            var xmlDocument = configRepository.GetXml();
            Assert.That(configRepository.HasChanged(), Is.EqualTo(true));
            // Test
            xmlDocument = configRepository.GetXml();
            Assert.That(configRepository.HasChanged(), Is.EqualTo(false));
            Assert.That(xmlDocument, Is.EqualTo("<root />"));
            File.Delete(diskPath);
        }
    }
}