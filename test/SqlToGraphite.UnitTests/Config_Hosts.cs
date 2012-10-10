using NUnit.Framework;
using Rhino.Mocks;
using SqlToGraphite.Config;
using SqlToGraphite.Plugin.SqlServer;

namespace SqlToGraphite.UnitTests
{
    [TestFixture]
    // ReSharper disable InconsistentNaming
    public class Config_Hosts
    {
        private string hostname1 = "hostname1";
        private string rolename1 = "roleName1";
        private string hostname2 = "hostname2";
        private string rolename2 = "roleName2";

        private SqlToGraphiteConfig config;

        private IAssemblyResolver assemblyResolver;

        [SetUp]
        public void SetUP()
        {
            this.assemblyResolver = MockRepository.GenerateMock<IAssemblyResolver>();
            this.config = new SqlToGraphiteConfig(this.assemblyResolver);
        }

        [Test]
        public void Should_not_validate_host_if_host_role_does_not_exist()
        {
            var h = new Host { Name = this.hostname1 };
            var r = new Role { Name = this.rolename1 };
            h.Roles.Add(r);
            this.config.Hosts.Add(h);
            //Test           
            var ex = Assert.Throws<RoleNotDefinedForHostException>(() => this.config.Validate());
            //Test
            Assert.That(ex.Message, Is.EqualTo(string.Format("The role {0} has not been defined for host {1}", this.rolename1, this.hostname1)));
        }

        [Test]
        public void Should_validate_job_if_client_exist()
        {
            var clientName = "cName";
            var c = new GraphiteTcpClient();
            c.ClientName = clientName;

            var jobName = "jobName";
            var job = new SqlServerClient();
            job.Name = jobName;
            job.ClientName = clientName;

            var wi = CreateWorkItems(jobName, this.rolename1, 100);
            var t = new Template();
            t.WorkItems.Add(wi);
            this.config.Templates.Add(t);
            this.config.Jobs.Add(job);
            //var job = new SqlServer { Name = this.rolename1 };
            //job.ClientName = clientName;
            var h = new Host { Name = this.hostname1 };
            var r = new Role { Name = this.rolename1 };
            h.Roles.Add(r);
            this.config.Clients.Add(c);
            this.config.Hosts.Add(h);
            //config.Jobs.Add(job);
            //Test
            this.config.Validate();
        }

        private static WorkItems CreateWorkItems(string jobName, string roleName, int freq)
        {
            var wi = new WorkItems { RoleName = roleName };
            var ts = new TaskSet { Frequency = freq };
            var task = new Task { JobName = jobName };
            ts.Tasks.Add(task);
            wi.TaskSet.Add(ts);
            return wi;
        }

        [Test]
        public void Should_add_single_host()
        {
            var h = new Host { Name = this.hostname1 };
            var r = new Role { Name = this.rolename1 };
            h.Roles.Add(r);
            this.config.Hosts.Add(h);
            //Test
            var sqlToGraphiteConfig = Helper.SerialiseDeserialise(this.config);
            //Assert
            Assert.That(sqlToGraphiteConfig.Hosts[0].Name, Is.EqualTo(this.hostname1));
            Assert.That(sqlToGraphiteConfig.Hosts[0].Roles[0].Name, Is.EqualTo(this.rolename1));
        }

        [Test]
        public void Should_add_multiple_hosts_of_same_type()
        {
            var h1 = new Host { Name = this.hostname1 };
            var h2 = new Host { Name = this.hostname2 };
            var r1 = new Role { Name = this.rolename1 };
            var r2 = new Role { Name = this.rolename2 };
            h1.Roles.Add(r1);
            this.config.Hosts.Add(h1);
            h2.Roles.Add(r2);
            this.config.Hosts.Add(h2);
            //Test
            var sqlToGraphiteConfig = Helper.SerialiseDeserialise(this.config);
            //Assert
            Assert.That(sqlToGraphiteConfig.Hosts[0].Name, Is.EqualTo(this.hostname1));
            Assert.That(sqlToGraphiteConfig.Hosts[0].Roles[0].Name, Is.EqualTo(this.rolename1));
            Assert.That(sqlToGraphiteConfig.Hosts[1].Name, Is.EqualTo(this.hostname2));
            Assert.That(sqlToGraphiteConfig.Hosts[1].Roles[0].Name, Is.EqualTo(this.rolename2));
        }
    }
}