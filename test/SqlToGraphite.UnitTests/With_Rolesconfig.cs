using System.Collections.Generic;
using NUnit.Framework;
using SqlToGraphite.Conf;
using SqlToGraphite.Config;

namespace SqlToGraphite.UnitTests
{
    using Rhino.Mocks;

    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class With_RolesConfig
    {
        private string hostName;

        private IEnvironment environment;

        [SetUp]
        public void SetUp()
        {            
            //this.hostName = Environment.MachineName;
            hostName = "someHost";
            environment = MockRepository.GenerateMock<IEnvironment>();
            environment.Expect(x => x.GetMachineName()).Return(hostName);            
        }

        [Test]
        public void Should_get_all_roles_for_default_only()
        {
            var sqlToGraphiteConfigHosts = new List<Host>();
            var host = new Host { Name = "default", Roles = new List<Role>() };
            host.Roles.Add(new Role { Name = "a1" });
            host.Roles.Add(new Role { Name = "a2" });
            sqlToGraphiteConfigHosts.Add(host);

            var roleConfig = new RoleConfig(sqlToGraphiteConfigHosts, environment);
            var roleList = roleConfig.GetRoleListToRunOnThisMachine();

            Assert.That(roleList.Count, Is.EqualTo(2));
            Assert.That(roleList[0], Is.EqualTo("a1"));
            Assert.That(roleList[1], Is.EqualTo("a2"));
        }

        [Test]
        public void Should_get_all_roles_for_default_and_not_host()
        {
            var sqlToGraphiteConfigHosts = new List<Host>();
            var hosta = new Host { Name = "default", Roles = new List<Role>() };
            var hostb = new Host { Name = "notThis", Roles = new List<Role>() };
            hosta.Roles.Add(new Role { Name = "a1" });
            hosta.Roles.Add(new Role { Name = "a2" });
            hostb.Roles.Add(new Role { Name = "b1" });

            sqlToGraphiteConfigHosts.Add(hosta);
            sqlToGraphiteConfigHosts.Add(hostb);
            var roleConfig = new RoleConfig(sqlToGraphiteConfigHosts, environment);
            var roleList = roleConfig.GetRoleListToRunOnThisMachine();

            Assert.That(roleList.Count, Is.EqualTo(2));
            Assert.That(roleList[0], Is.EqualTo("a1"));
            Assert.That(roleList[1], Is.EqualTo("a2"));
        }

        [Test]
        public void Should_get_all_roles_for_default_and_host()
        {
            var sqlToGraphiteConfigHosts = new List<Host>();
            var hosta = new Host { Name = "default", Roles = new List<Role>() };
            var hostb = new Host { Name = "notThis", Roles = new List<Role>() };
            var hostc = new Host { Name = hostName, Roles = new List<Role>() };
            hosta.Roles.Add(new Role { Name = "a1" });
            hosta.Roles.Add(new Role { Name = "a2" });
            hostb.Roles.Add(new Role { Name = "b1" });
            hostc.Roles.Add(new Role { Name = "c1" });
            
            sqlToGraphiteConfigHosts.Add(hosta);
            sqlToGraphiteConfigHosts.Add(hostb);
            sqlToGraphiteConfigHosts.Add(hostc);
            var roleConfig = new RoleConfig(sqlToGraphiteConfigHosts, environment);
            var roleList = roleConfig.GetRoleListToRunOnThisMachine();

            Assert.That(roleList.Count, Is.EqualTo(3));
            Assert.That(roleList[0], Is.EqualTo("a1"));
            Assert.That(roleList[1], Is.EqualTo("a2"));
            Assert.That(roleList[2], Is.EqualTo("c1"));
        }

        [Test]
        public void Should_get_all_roles_for_default_and_regex_of_the_machines_hostname_direct_match()
        {
            var machineName = "abc";
            var hname = "abc";
            var sqlToGraphiteConfigHosts = new List<Host>();
            var hosta = new Host { Name = "default", Roles = new List<Role>() };
            var hostb = new Host { Name = "notThis", Roles = new List<Role>() };
            var hostc = new Host { Name = hname, Roles = new List<Role>() };

            hosta.Roles.Add(new Role { Name = "a1" });
            hosta.Roles.Add(new Role { Name = "a2" });
            hostb.Roles.Add(new Role { Name = "b1" });
            hostc.Roles.Add(new Role { Name = "c1" });

            sqlToGraphiteConfigHosts.Add(hosta);
            sqlToGraphiteConfigHosts.Add(hostb);
            sqlToGraphiteConfigHosts.Add(hostc);

            environment = MockRepository.GenerateMock<IEnvironment>();
            environment.Expect(x => x.GetMachineName()).Return(machineName); 
            var roleConfig = new RoleConfig(sqlToGraphiteConfigHosts, environment);
            var roleList = roleConfig.GetRoleListToRunOnThisMachine();

            Assert.That(roleList.Count, Is.EqualTo(3));
            Assert.That(roleList[0], Is.EqualTo("a1"));
            Assert.That(roleList[1], Is.EqualTo("a2"));
            Assert.That(roleList[2], Is.EqualTo("c1"));
        }

        [Test]
        public void Should_get_all_roles_for_default_and_regex_of_the_machines_hostname_regex_match()
        {
            var machineName = "lonbti-bus01v";
            var hname = ".*bus.*";
            var sqlToGraphiteConfigHosts = new List<Host>();
            var hosta = new Host { Name = "default", Roles = new List<Role>() };
            var hostb = new Host { Name = "notThis", Roles = new List<Role>() };
            var hostc = new Host { Name = hname, Roles = new List<Role>() };
            hosta.Roles.Add(new Role { Name = "a1" });
            hosta.Roles.Add(new Role { Name = "a2" });
            hostb.Roles.Add(new Role { Name = "b1" });
            hostc.Roles.Add(new Role { Name = "c1" });
            sqlToGraphiteConfigHosts.Add(hosta);
            sqlToGraphiteConfigHosts.Add(hostb);
            sqlToGraphiteConfigHosts.Add(hostc);
            environment = MockRepository.GenerateMock<IEnvironment>();
            environment.Expect(x => x.GetMachineName()).Return(machineName); 
            var roleConfig = new RoleConfig(sqlToGraphiteConfigHosts, environment);
            var roleList = roleConfig.GetRoleListToRunOnThisMachine();

            Assert.That(roleList.Count, Is.EqualTo(3));
            Assert.That(roleList[0], Is.EqualTo("a1"));
            Assert.That(roleList[1], Is.EqualTo("a2"));
            Assert.That(roleList[2], Is.EqualTo("c1"));
        }

        [Test]
        public void Should_get_all_roles_for_default_and_two_host_list()
        {
            var sqlToGraphiteConfigHosts = new List<Host>();
            var hosta = new Host { Name = "default", Roles = new List<Role>() };
            var hostb = new Host { Name = "notThis", Roles = new List<Role>() };
            var hostc = new Host { Name = hostName, Roles = new List<Role>() };
            hosta.Roles.Add(new Role { Name = "a1" });
            hosta.Roles.Add(new Role { Name = "a2" });
            hostb.Roles.Add(new Role { Name = "b1" });
            hostc.Roles.Add(new Role { Name = "c1" });
            hostc.Roles.Add(new Role { Name = "c2" }); 

            sqlToGraphiteConfigHosts.Add(hosta);
            sqlToGraphiteConfigHosts.Add(hostb);
            sqlToGraphiteConfigHosts.Add(hostc);

            var roleConfig = new RoleConfig(sqlToGraphiteConfigHosts, environment);
            var roleList = roleConfig.GetRoleListToRunOnThisMachine();

            Assert.That(roleList.Count, Is.EqualTo(4));
            Assert.That(roleList[0], Is.EqualTo("a1"));
            Assert.That(roleList[1], Is.EqualTo("a2"));
            Assert.That(roleList[2], Is.EqualTo("c1"));
            Assert.That(roleList[3], Is.EqualTo("c2"));
        }
    }
}