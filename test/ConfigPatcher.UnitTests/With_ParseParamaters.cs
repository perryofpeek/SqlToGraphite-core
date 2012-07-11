using NUnit.Framework;

namespace ConfigPatcher.UnitTests
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class With_ParseParamaters
    {
        [Test]
        public void Should_get_username_and_password_lowercase()
        {
            var username = "someName";
            var password = "somePass";

            var input = new string[2];
            input[0] = string.Format("username={0}", username);
            input[1] = string.Format("password={0}", password);
            var parseParamaters = new ParseParamaters(input);
            Assert.That(parseParamaters.Username, Is.EqualTo(username));
            Assert.That(parseParamaters.Password, Is.EqualTo(password));
        }

        [Test]
        public void Should_get_all_lowercase()
        {
            var username = "someName";
            var password = "somePass";
            var hostname = "hostname";
            var configUpdate = "5";
            var configRetry = "6";
            var cacheLength = "7";
            var path = "somePath";
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
            Assert.That(parseParamaters.Username, Is.EqualTo(username));
            Assert.That(parseParamaters.Password, Is.EqualTo(password));
            Assert.That(parseParamaters.Hostname, Is.EqualTo(hostname));
            Assert.That(parseParamaters.ConfigUpdate, Is.EqualTo(configUpdate));
            Assert.That(parseParamaters.ConfigRetry, Is.EqualTo(configRetry));
            Assert.That(parseParamaters.CacheLength, Is.EqualTo(cacheLength));
            Assert.That(parseParamaters.ConfigUri, Is.EqualTo(uri));
            Assert.That(parseParamaters.Path, Is.EqualTo(path));

        }

        [Test]
        public void Should_get_username_lowercase()
        {
            var username = "someName";

            var input = new string[1];
            input[0] = string.Format("username={0}", username);

            var parseParamaters = new ParseParamaters(input);
            Assert.That(parseParamaters.Username, Is.EqualTo(username));
        }

        [Test]
        public void Should_get_username_not_lowercase()
        {
            var username = "someName";

            var input = new string[1];
            input[0] = string.Format("username={0}", username);

            var parseParamaters = new ParseParamaters(input);
            Assert.That(parseParamaters.Username, Is.EqualTo(username));
        }

        [Test]
        public void Should_not_get_username_not_defined()
        {
            var input = new string[1];
            input[0] = "UseRName";

            var parseParamaters = new ParseParamaters(input);
            Assert.That(parseParamaters.Username, Is.EqualTo(string.Empty));
        }

        [Test]
        public void Should_check_default_values()
        {
            var input = new string[1];
            var parseParamaters = new ParseParamaters(input);
            Assert.That(parseParamaters.Username, Is.EqualTo(string.Empty));
            Assert.That(parseParamaters.Password, Is.EqualTo(string.Empty));
            Assert.That(parseParamaters.Hostname, Is.EqualTo(string.Empty));
            Assert.That(parseParamaters.ConfigUpdate, Is.EqualTo("10"));
            Assert.That(parseParamaters.ConfigRetry, Is.EqualTo("10"));
            Assert.That(parseParamaters.CacheLength, Is.EqualTo("10"));
        }
    }
}
