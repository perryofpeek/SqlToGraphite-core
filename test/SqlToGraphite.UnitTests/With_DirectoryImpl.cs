using System.IO;

using NUnit.Framework;

namespace SqlToGraphite.UnitTests
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class With_DirectoryImpl
    {
        [Test]
        public void Should_Get_single_file()
        {
            File.WriteAllText("a.abc", "123");
            var dir = new DirectoryImpl();
            var files = dir.GetFilesInCurrentDirectory("*.abc");
            Assert.That(files.Count, Is.EqualTo(1));
            File.Delete("a.abc");
        }

        [Test]
        public void Should_Get_multiple_files()
        {
            File.WriteAllText("a.abc", "123");
            File.WriteAllText("b.abc", "123");
            var dir = new DirectoryImpl();
            var files = dir.GetFilesInCurrentDirectory("*.abc");
            Assert.That(files.Count, Is.EqualTo(2));
            File.Delete("a.abc");
            File.Delete("b.abc");
        }

        [Test]
        public void Should_Get_zero_files()
        {            
            var dir = new DirectoryImpl();
            var files = dir.GetFilesInCurrentDirectory("*.xyz");
            Assert.That(files.Count, Is.EqualTo(0));
        }
    }
}