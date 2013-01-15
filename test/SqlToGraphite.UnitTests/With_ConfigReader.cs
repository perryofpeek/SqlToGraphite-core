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
        private ConfigHttpReader configHttpRepository;

        private string diskPath;

        [SetUp]
        public void SetUp()
        {
            var filename = Guid.NewGuid().ToString();
            diskPath = @"c:\" + filename;
            var uriPath = "file:///C:/" + filename;
            this.configHttpRepository = new ConfigHttpReader(uriPath, string.Empty, string.Empty);
        }

        [Test]
        public void Should_get_config()
        {
            File.WriteAllText(diskPath, "test123");
            var s = this.configHttpRepository.GetXml();
            Assert.That(s, Is.EqualTo("test123"));
            File.Delete(diskPath);
        }

        [Test]
        public void Should_get_config_as_Xml_Doc()
        {
            File.WriteAllText(diskPath, "<root />");
            var xmlDocument = this.configHttpRepository.GetXml();
            Assert.That(xmlDocument, Is.EqualTo("<root />"));
            File.Delete(diskPath);
        }

        [Test]
        public void Should_return_true_if_config_has_changed()
        {
            File.WriteAllText(diskPath, "<root />");
            var xmlDocument = this.configHttpRepository.GetXml();
            Assert.That(this.configHttpRepository.HasChanged(), Is.EqualTo(true));
            Assert.That(xmlDocument, Is.EqualTo("<root />"));
            File.Delete(diskPath);
        }

        [Test]
        public void Should_return_false_if_config_has_changed()
        {
            File.WriteAllText(diskPath, "<root />");
            var xmlDocument = this.configHttpRepository.GetXml();
            Assert.That(this.configHttpRepository.HasChanged(), Is.EqualTo(true));
            // Test
            xmlDocument = this.configHttpRepository.GetXml();
            Assert.That(this.configHttpRepository.HasChanged(), Is.EqualTo(false));
            Assert.That(xmlDocument, Is.EqualTo("<root />"));
            File.Delete(diskPath);
        }

        [Test]
        public void Should_return_hash_of_xml()
        {
            File.WriteAllText(diskPath, "<root />");
            var xmlDocument = this.configHttpRepository.GetXml();
            Assert.That(this.configHttpRepository.HasChanged(), Is.EqualTo(true));
            // Test
            xmlDocument = this.configHttpRepository.GetXml();
            Assert.That(this.configHttpRepository.GetHash(), Is.EqualTo("EDD2208619E0E0884A536014611A1FD4"));
            File.Delete(diskPath);
        }
    }
}