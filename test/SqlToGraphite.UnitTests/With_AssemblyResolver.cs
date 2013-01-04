using System.Collections.Generic;
using System.IO;
using log4net;
using NUnit.Framework;
using Rhino.Mocks;
using SqlToGraphiteInterfaces;

namespace SqlToGraphite.UnitTests
{
    using SqlToGraphite.Plugin.Wmi;

    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class With_AssemblyResolver
    {
        private IDirectory dir;

        private ILog log;

        private AssemblyResolver assemblyResolver;

        [SetUp]
        public void SetUp()
        {
            dir = MockRepository.GenerateMock<IDirectory>();
            log = MockRepository.GenerateMock<ILog>();
        }

        [Test]
        public void Should_Resolve_type_sucessfuly()
        {
            var job = new WmiClient();
            var files = new List<string>() { string.Format(@"{0}\SqlToGraphite.Plugin.Wmi.dll", Directory.GetCurrentDirectory()) };
            dir.Expect(x => x.GetFilesInCurrentDirectory(AssemblyResolver.FilesToScan)).Return(files);
            //Test
            assemblyResolver = new AssemblyResolver(this.dir, log);
            var rtn = assemblyResolver.ResolveType(job);
            //Assert
            Assert.That(rtn.FullName, Is.EqualTo(job.GetType().FullName));
            dir.VerifyAllExpectations();
        }

        [Test]
        public void Should_exception_if_type_not_found()
        {
            var job = new PrivateJob { Type = "someType" };
            assemblyResolver = new AssemblyResolver(this.dir, log);
            //Test
            var ex = Assert.Throws<PluginNotFoundOrLoadedException>(() => assemblyResolver.ResolveType(job));
            //Assert
            Assert.That(ex.Message, Is.EqualTo(string.Format("The plugin someType is not found or loaded")));
        }

        [Test]
        public void Should_ignore_bad_image_exception_for_not_dot_net_dlls()
        {
            File.WriteAllText("bad.dll", "abc");
            var job = new WmiClient() { Type = "SqlToGraphite.Plugin.Wmi.WmiClient" };
            var files = new List<string>() { string.Format(@"{0}\bad.dll", Directory.GetCurrentDirectory()), string.Format(@"{0}\SqlToGraphite.Plugin.Wmi.dll", Directory.GetCurrentDirectory()) };
            dir.Expect(x => x.GetFilesInCurrentDirectory(AssemblyResolver.FilesToScan)).Return(files);
            //Test
            assemblyResolver = new AssemblyResolver(this.dir, log);
            //Test
            var type = assemblyResolver.ResolveType(job);
            //Assert
            Assert.That(type.FullName, Is.EqualTo(job.GetType().FullName));
            File.Delete("bad.dll");
        }

        internal class PrivateJob : Job
        {
            public override string Name { get; set; }

            public override string ClientName { get; set; }

            public override string Type { get; set; }

            public override IList<IResult> Get()
            {
                throw new System.NotImplementedException();
            }
        }
    }
}