using NUnit.Framework;

namespace SqlToGraphite.UnitTests
{
    using System;

    using Graphite;

    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class With_GraphiteMetric
    {
        [Test]
        public void Should_get_a_graphite_object_passing_in_int()
        {
            var timestamp = new DateTime(2012, 11, 10, 9, 8, 7);
            var path = "path";
            int value = 123;
            var unixtime = 1352538487;
            IGraphiteMetric graphiteMetric = new GraphiteMetric(path, value, timestamp);

            Assert.That(graphiteMetric.Path, Is.EqualTo(path));
            Assert.That(graphiteMetric.Value, Is.EqualTo(value.ToString()));
            Assert.That(graphiteMetric.DotNetTimeStamp, Is.EqualTo(timestamp));
            Assert.That(graphiteMetric.UnixTimeStamp, Is.EqualTo(unixtime));
            Assert.That(graphiteMetric.ToGraphiteMessage(), Is.EqualTo(string.Format("{0} {1} {2}\n", path, value, unixtime)));
        }

        [Test]
        public void Should_get_a_graphite_object_passing_in_long()
        {
            var timestamp = new DateTime(2012, 11, 10, 9, 8, 7);
            var path = "path";
            long value = 121323;
            var unixtime = 1352538487;
            IGraphiteMetric graphiteMetric = new GraphiteMetric(path, value, timestamp);

            Assert.That(graphiteMetric.Path, Is.EqualTo(path));
            Assert.That(graphiteMetric.Value, Is.EqualTo(value.ToString()));
            Assert.That(graphiteMetric.DotNetTimeStamp, Is.EqualTo(timestamp));
            Assert.That(graphiteMetric.UnixTimeStamp, Is.EqualTo(unixtime));
            Assert.That(graphiteMetric.ToGraphiteMessage(), Is.EqualTo(string.Format("{0} {1} {2}\n", path, value, unixtime)));
        }

        [Test]
        public void Should_get_a_graphite_object_passing_in_float()
        {
            var timestamp = new DateTime(2012, 11, 10, 9, 8, 7);
            var path = "path";
            float value = 1213231;
            var unixtime = 1352538487;
            IGraphiteMetric graphiteMetric = new GraphiteMetric(path, value, timestamp);

            Assert.That(graphiteMetric.Path, Is.EqualTo(path));
            Assert.That(graphiteMetric.Value, Is.EqualTo(value.ToString()));
            Assert.That(graphiteMetric.DotNetTimeStamp, Is.EqualTo(timestamp));
            Assert.That(graphiteMetric.UnixTimeStamp, Is.EqualTo(unixtime));
            Assert.That(graphiteMetric.ToGraphiteMessage(), Is.EqualTo(string.Format("{0} {1} {2}\n", path, value, unixtime)));
        }

        [Test]
        public void Should_get_a_graphite_object_passing_in_double()
        {
            var timestamp = new DateTime(2012, 11, 10, 9, 8, 7);
            var path = "path";
            double value = 1213231;
            var unixtime = 1352538487;
            IGraphiteMetric graphiteMetric = new GraphiteMetric(path, value, timestamp);

            Assert.That(graphiteMetric.Path, Is.EqualTo(path));
            Assert.That(graphiteMetric.Value, Is.EqualTo(value.ToString()));
            Assert.That(graphiteMetric.DotNetTimeStamp, Is.EqualTo(timestamp));
            Assert.That(graphiteMetric.UnixTimeStamp, Is.EqualTo(unixtime));
            Assert.That(graphiteMetric.ToGraphiteMessage(), Is.EqualTo(string.Format("{0} {1} {2}\n", path, value, unixtime)));
        }

        [Test]
        public void Should_get_a_graphite_object_passing_in_decimal()
        {
            var timestamp = new DateTime(2012, 11, 10, 9, 8, 7);
            var path = "path";
            decimal value = -16325.62m;
            var unixtime = 1352538487;
            IGraphiteMetric graphiteMetric = new GraphiteMetric(path, value, timestamp);

            Assert.That(graphiteMetric.Path, Is.EqualTo(path));
            Assert.That(graphiteMetric.Value, Is.EqualTo(value.ToString()));
            Assert.That(graphiteMetric.DotNetTimeStamp, Is.EqualTo(timestamp));
            Assert.That(graphiteMetric.UnixTimeStamp, Is.EqualTo(unixtime));
            Assert.That(graphiteMetric.ToGraphiteMessage(), Is.EqualTo(string.Format("{0} {1} {2}\n", path, value, unixtime)));
        }
    }
}