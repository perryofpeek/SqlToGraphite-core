using NUnit.Framework;

namespace SqlToGraphite.UnitTests
{
    using System;
    using System.Collections.Generic;

    using Graphite;

    using SqlToGraphiteInterfaces;

    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class With_GraphiteMetrics
    {
        [Test]
        public void Should_return_a_graphite_message_list()
        {
            var list = new List<IResult>();
            var res = new Result("name", new DateTime(2012, 12, 11, 10, 9, 8), "path");
            res.SetValue(123);
            list.Add(res);
            var graphiteMetrics = new GraphiteMetrics(list);
            var msgList = graphiteMetrics.ToGraphiteMessageList();
            string msg = "path.name 123 1355220548\n";
            Assert.That(msgList, Is.EqualTo(msg));
        }
    }
}