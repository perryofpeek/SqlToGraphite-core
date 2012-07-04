using System;

using NUnit.Framework;

using SqlToGraphiteInterfaces;

namespace SqlToGraphite.UnitTests
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class With_result
    {
        [Test]
        public void Should_create_result_using_constructor()
        {
            const int Value = 13;
            var d = DateTime.Now;
            var name = "name";
            var path = "path";
            var result = new Result(Value, name, d, path);
            Assert.That(result.Value, Is.EqualTo(Value));
            Assert.That(result.Name, Is.EqualTo(name));
            Assert.That(result.Path, Is.EqualTo(path));
            Assert.That(result.TimeStamp.Day, Is.EqualTo(DateTime.Now.Day));
            Assert.That(result.TimeStamp.Hour, Is.EqualTo(DateTime.Now.Hour));
            Assert.That(result.TimeStamp.Minute, Is.EqualTo(DateTime.Now.Minute));
            Assert.That(result.TimeStamp.Month, Is.EqualTo(DateTime.Now.Month));
        }

        [Test]
        public void Should_create_result_with_integer_and_date()
        {
            const int Value = 13;
            var name = "name";
            var date = new DateTime(2001, 12, 11, 9, 8, 7);
            var result = new Result(Value, name, date, "path");
            Assert.That(result.Value, Is.EqualTo(Value));
            Assert.That(result.TimeStamp, Is.EqualTo(date));
            Assert.That(result.Name, Is.EqualTo(name));
        }

        [Test]
        public void Should_create_result_with_integer_and_date_and_name()
        {
            const int Value = 13;
            var name = "name";
            var date = new DateTime(2001, 12, 11, 9, 8, 7);
            var result = new Result(Value, name, date, "path");
            Assert.That(result.Value, Is.EqualTo(Value));
            Assert.That(result.TimeStamp, Is.EqualTo(date));
            Assert.That(result.Name, Is.EqualTo(name));
        }

        [Test]
        public void Should_create_result_with_integer_and_date_and_name_remove_special_characters()
        {
            const int Value = 13;
            var name = "D:";
            var date = new DateTime(2001, 12, 11, 9, 8, 7);
            var result = new Result(Value, name, date, "path");
            Assert.That(result.Value, Is.EqualTo(Value));
            Assert.That(result.TimeStamp, Is.EqualTo(date));
            Assert.That(result.Name, Is.EqualTo("D"));
        }

        [Test]
        public void Should_create_full_path()
        {
            const int Value = 13;
            var name = "D:";
            var date = new DateTime(2001, 12, 11, 9, 8, 7);
            var result = new Result(Value, name, date, "path");
            Assert.That(result.FullPath, Is.EqualTo("path.D"));
        }

        [Test]
        public void Should_create_full_path_replace_spaces_with_underscore()
        {
            const int Value = 13;
            var name = "name";
            var date = new DateTime(2001, 12, 11, 9, 8, 7);
            var result = new Result(Value, name, date, "some.path.with a space");
            Assert.That(result.FullPath, Is.EqualTo("some.path.with_a_space.name"));
        }

        [Test]
        public void Should_create_insert_hostname_into_full_path()
        {
            const int Value = 13;
            var machinecName = Environment.MachineName;
            var name = "D:";
            var date = new DateTime(2001, 12, 11, 9, 8, 7);
            var result = new Result(Value, name, date, "path.%h");
            Assert.That(result.FullPath, Is.EqualTo(string.Format("path.{0}.D", machinecName)));
        }
    }
}