using System;
using System.Collections.Generic;

using NUnit.Framework;

using SqlToGraphite.Conf;

namespace SqlToGraphite.UnitTests
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class With_RolesConfig
    {
        private string hostName;

        [SetUp]
        public void SetUp()
        {
            this.hostName = Environment.MachineName;
        }

        [Test]
        public void Should_get_all_roles_for_default_only()
        {
            var sqlToGraphiteConfigHosts = new List<SqlToGraphiteConfigHostsHost>();
            var host = new SqlToGraphiteConfigHostsHost { name = "default", role = new SqlToGraphiteConfigHostsHostRole[2] };
            host.role[0] = new SqlToGraphiteConfigHostsHostRole { name = "a1" };
            host.role[1] = new SqlToGraphiteConfigHostsHostRole { name = "a2" };
            sqlToGraphiteConfigHosts.Add(host);

            var roleConfig = new RoleConfig(sqlToGraphiteConfigHosts);
            var roleList = roleConfig.GetRoleList();

            Assert.That(roleList.Count, Is.EqualTo(2));
            Assert.That(roleList[0], Is.EqualTo("a1"));
            Assert.That(roleList[1], Is.EqualTo("a2"));
        }

        [Test]
        public void Should_get_all_roles_for_default_and_not_host()
        {
            var sqlToGraphiteConfigHosts = new List<SqlToGraphiteConfigHostsHost>();
            var hosta = new SqlToGraphiteConfigHostsHost { name = "default", role = new SqlToGraphiteConfigHostsHostRole[2] };
            var hostb = new SqlToGraphiteConfigHostsHost { name = "notThis", role = new SqlToGraphiteConfigHostsHostRole[1] };
            hosta.role[0] = new SqlToGraphiteConfigHostsHostRole { name = "a1" };
            hosta.role[1] = new SqlToGraphiteConfigHostsHostRole { name = "a2" };
            hostb.role[0] = new SqlToGraphiteConfigHostsHostRole { name = "b1" };

            sqlToGraphiteConfigHosts.Add(hosta);
            sqlToGraphiteConfigHosts.Add(hostb);
            var roleConfig = new RoleConfig(sqlToGraphiteConfigHosts);
            var roleList = roleConfig.GetRoleList();

            Assert.That(roleList.Count, Is.EqualTo(2));
            Assert.That(roleList[0], Is.EqualTo("a1"));
            Assert.That(roleList[1], Is.EqualTo("a2"));
        }

        [Test]
        public void Should_get_all_roles_for_default_and_host()
        {
            var sqlToGraphiteConfigHosts = new List<SqlToGraphiteConfigHostsHost>();
            var hosta = new SqlToGraphiteConfigHostsHost { name = "default", role = new SqlToGraphiteConfigHostsHostRole[2] };
            var hostb = new SqlToGraphiteConfigHostsHost { name = "notThis", role = new SqlToGraphiteConfigHostsHostRole[1] };
            var hostc = new SqlToGraphiteConfigHostsHost { name = hostName, role = new SqlToGraphiteConfigHostsHostRole[1] };
            hosta.role[0] = new SqlToGraphiteConfigHostsHostRole { name = "a1" };
            hosta.role[1] = new SqlToGraphiteConfigHostsHostRole { name = "a2" };
            hostb.role[0] = new SqlToGraphiteConfigHostsHostRole { name = "b1" };
            hostc.role[0] = new SqlToGraphiteConfigHostsHostRole { name = "c1" };

            sqlToGraphiteConfigHosts.Add(hosta);
            sqlToGraphiteConfigHosts.Add(hostb);
            sqlToGraphiteConfigHosts.Add(hostc);
            var roleConfig = new RoleConfig(sqlToGraphiteConfigHosts);
            var roleList = roleConfig.GetRoleList();

            Assert.That(roleList.Count, Is.EqualTo(3));
            Assert.That(roleList[0], Is.EqualTo("a1"));
            Assert.That(roleList[1], Is.EqualTo("a2"));
            Assert.That(roleList[2], Is.EqualTo("c1"));
        }

        [Test]
        public void Should_get_all_roles_for_default_and_two_host_list()
        {
            var sqlToGraphiteConfigHosts = new List<SqlToGraphiteConfigHostsHost>();
            var hosta = new SqlToGraphiteConfigHostsHost { name = "default", role = new SqlToGraphiteConfigHostsHostRole[2] };
            var hostb = new SqlToGraphiteConfigHostsHost { name = "notThis", role = new SqlToGraphiteConfigHostsHostRole[1] };
            var hostc = new SqlToGraphiteConfigHostsHost { name = hostName, role = new SqlToGraphiteConfigHostsHostRole[2] };
            hosta.role[0] = new SqlToGraphiteConfigHostsHostRole { name = "a1" };
            hosta.role[1] = new SqlToGraphiteConfigHostsHostRole { name = "a2" };
            hostb.role[0] = new SqlToGraphiteConfigHostsHostRole { name = "b1" };
            hostc.role[0] = new SqlToGraphiteConfigHostsHostRole { name = "c1" };
            hostc.role[1] = new SqlToGraphiteConfigHostsHostRole { name = "c2" };

            sqlToGraphiteConfigHosts.Add(hosta);
            sqlToGraphiteConfigHosts.Add(hostb);
            sqlToGraphiteConfigHosts.Add(hostc);

            var roleConfig = new RoleConfig(sqlToGraphiteConfigHosts);
            var roleList = roleConfig.GetRoleList();

            Assert.That(roleList.Count, Is.EqualTo(4));
            Assert.That(roleList[0], Is.EqualTo("a1"));
            Assert.That(roleList[1], Is.EqualTo("a2"));
            Assert.That(roleList[2], Is.EqualTo("c1"));
            Assert.That(roleList[3], Is.EqualTo("c2"));
        }
    }
}