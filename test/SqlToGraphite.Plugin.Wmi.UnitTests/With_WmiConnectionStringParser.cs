using NUnit.Framework;

using SqlToGraphite.Plugin.Wmi;

namespace SqlToGrpahite.Plugin.Wmi.UnitTests
{
    // ReSharper disable InconsistentNaming
    [TestFixture, Ignore()]
    public class With_WmiConnectionStringParser
    {
        [Test]
        public void Should_split_connection_string_into_username()
        {
            var username = "user1";
            var connectionString = string.Format("Username={0};Password={1};", username, string.Empty);
            var wmiCs = new WmiConnectionStringParser(connectionString);
            Assert.That(wmiCs.Username, Is.EqualTo(username));
        }

        [Test]
        public void Should_split_connection_string_into_password()
        {
            var username = "user1";
            var password = "password1";
            var connectionString = string.Format("Username={0};Password={1};", username, password);
            var wmiCs = new WmiConnectionStringParser(connectionString);
            Assert.That(wmiCs.Password, Is.EqualTo(password));
        }

        [Test]
        public void Should_split_connection_string_into_hostname()
        {
            var username = "user1";
            var password = "password1";
            var hostname = "someHost";
            var connectionString = string.Format("Username={0};Hostname={2};Password={1};", username, password, hostname);
            var wmiCs = new WmiConnectionStringParser(connectionString);
            Assert.That(wmiCs.Hostname, Is.EqualTo(hostname));
        }

        [Test]
        public void Should_split_connection_string_into_username_and_password()
        {
            var username = "user1";
            var password = "password1";
            var connectionString = string.Format("Username={0};Password={1};", username, password);
            var wmiCs = new WmiConnectionStringParser(connectionString);
            Assert.That(wmiCs.Password, Is.EqualTo(password));
            Assert.That(wmiCs.Username, Is.EqualTo(username));
        }

        [Test]
        public void Should_split_connection_string_into_username_and_password_one_semicolon()
        {
            var username = "user1";
            var password = "password1";
            var connectionString = string.Format("Username={0};Password={1}", username, password);
            var wmiCs = new WmiConnectionStringParser(connectionString);
            Assert.That(wmiCs.Password, Is.EqualTo(password));
            Assert.That(wmiCs.Username, Is.EqualTo(username));
        }

        [Test]
        public void Should_only_set_username_missing_semicolons()
        {
            var username = "user1";
            var password = "password1";
            var connectionString = string.Format("Username={0}Password={1}", username, password);
            var wmiCs = new WmiConnectionStringParser(connectionString);
            Assert.That(wmiCs.Password, Is.EqualTo(string.Empty));
            Assert.That(wmiCs.Username, Is.EqualTo("user1Password"));
        }

        [Test]
        public void Should_only_set_password_missing_username_name()
        {
            var username = "user1";
            var password = "password1";
            var connectionString = string.Format("sername={0};Password={1}", username, password);
            var wmiCs = new WmiConnectionStringParser(connectionString);
            Assert.That(wmiCs.Password, Is.EqualTo(password));
            Assert.That(wmiCs.Username, Is.EqualTo(string.Empty));
        }
    }
}