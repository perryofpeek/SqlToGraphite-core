using System.IO;
using System.Xml;

using NUnit.Framework;

namespace ConfigPatcher.UnitTests
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class With_patch
    {
        [SetUp]
        public void SetUp()
        {
            File.Copy("orig.config", "test.config", true);
        }


        [Test]
        public void Should_not_add_username_when_patching_config_file()
        {
            var username = "";
            var password = "somePass";
            var input = new string[3];
            input[0] = string.Format("username={0}", username);
            input[1] = string.Format("password={0}", password);
            input[2] = string.Format("path={0}", "test.config");

            var parseParamaters = new ParseParamaters(input);

            var patcher = new Patcher(parseParamaters);
            patcher.Patch();

            var xmlDocument = new XmlDocument();
            xmlDocument.Load(@"test.config");

            Assert.That(xmlDocument.SelectSingleNode("/configuration/SqlToGraphite/@ConfigUsername"), Is.Null);
            Assert.That(xmlDocument.SelectSingleNode("/configuration/SqlToGraphite/@ConfigPassword").Value, Is.EqualTo(password));
        }

        [Test]
        public void Should_not_add_password_when_patching_config_file()
        {
            var username = "uname";
            var password = "";
            var input = new string[3];
            input[0] = string.Format("username={0}", username);
            input[1] = string.Format("password={0}", password);
            input[2] = string.Format("path={0}", "test.config");
            var parseParamaters = new ParseParamaters(input);
            var patcher = new Patcher(parseParamaters);
            patcher.Patch();
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(@"test.config");
            Assert.That(xmlDocument.SelectSingleNode("/configuration/SqlToGraphite/@ConfigUsername").Value, Is.EqualTo(username));
            Assert.That(xmlDocument.SelectSingleNode("/configuration/SqlToGraphite/@ConfigPassword"), Is.Null);
        }

        [Test]
        public void Should_not_add_password_or_username_when_patching_config_file()
        {
            var username = "";
            var password = "";
            var input = new string[3];
            input[0] = string.Format("username={0}", username);
            input[1] = string.Format("password={0}", password);
            input[2] = string.Format("path={0}", "test.config");
            var parseParamaters = new ParseParamaters(input);
            var patcher = new Patcher(parseParamaters);
            patcher.Patch();
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(@"test.config");
            Assert.That(xmlDocument.SelectSingleNode("/configuration/SqlToGraphite/@ConfigUsername"), Is.Null);
            Assert.That(xmlDocument.SelectSingleNode("/configuration/SqlToGraphite/@ConfigPassword"), Is.Null);
        }



        [Test]
        public void Should_patch_config_file()
        {
            var username = "someName";
            var password = "somePass";
            var hostname = "hostname";
            var configUpdate = "5";
            var configRetry = "6";
            var cacheLength = "7";
            var path = "test.config";
            var uri = "uri";

            var input = new string[8];
            input[0] = string.Format("username={0}", username);
            input[1] = string.Format("password={0}", password);
            input[2] = string.Format("hostname={0}", hostname);
            input[3] = string.Format("configupdate={0}", configUpdate);
            input[4] = string.Format("configretry={0}", configRetry);
            input[5] = string.Format("cachelength={0}", cacheLength);
            input[6] = string.Format("path={0}", path);
            input[7] = string.Format("configuri={0}", uri);


            var parseParamaters = new ParseParamaters(input);

            var patcher = new Patcher(parseParamaters);
            patcher.Patch();

            var xmlDocument = new XmlDocument();
            xmlDocument.Load(@"test.config");

            Assert.That(xmlDocument.SelectSingleNode("/configuration/SqlToGraphite/@Hostname").Value, Is.EqualTo(hostname));
            Assert.That(xmlDocument.SelectSingleNode("/configuration/SqlToGraphite/@ConfigUsername").Value, Is.EqualTo(username));
            Assert.That(xmlDocument.SelectSingleNode("/configuration/SqlToGraphite/@ConfigPassword").Value, Is.EqualTo(password));
            Assert.That(xmlDocument.SelectSingleNode("/configuration/SqlToGraphite/@ConfigUri").Value, Is.EqualTo(uri));
            Assert.That(xmlDocument.SelectSingleNode("/configuration/SqlToGraphite/@CheckConfigUpdatedEveryMinutes").Value, Is.EqualTo(configUpdate));
            Assert.That(xmlDocument.SelectSingleNode("/configuration/SqlToGraphite/@MinutesBetweenRetryToGetConfigOnError").Value, Is.EqualTo(configRetry));
            Assert.That(xmlDocument.SelectSingleNode("/configuration/SqlToGraphite/@ConfigCacheLengthMinutes").Value, Is.EqualTo(cacheLength));
        }
    }
}
