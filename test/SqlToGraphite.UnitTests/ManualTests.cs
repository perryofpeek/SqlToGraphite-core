using System;
using System.Collections.Generic;
using log4net;
using NUnit.Framework;
using SqlToGraphite.Plugin.SqlServer;
using SqlToGraphiteInterfaces;

namespace SqlToGraphite.UnitTests
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class ManualTests
    {
        [Test, Explicit]
        public void Should_()
        {
            //var log = LogManager.GetLogger("log");
            //log4net.Config.XmlConfigurator.Configure();
            //var host = "metrics.london.ttldev.local";
            //var port = 2003;
            //var connectionString = "Data Source=nuget.london.ttldev.local;Initial Catalog=GoReportingSvn3;User Id=sa;Password=!bcde1234;";
            //var sqlGetter = new SqlGetter(connectionString, host, port, log);
            //var sql = "SELECT [Assigned],DateDiff(s,[Scheduled],[Assigned]) FROM [GoReportingSvn3].[dbo].[V_BuildStateTransition] where Scheduled > '2012-01-01'";
            //sqlGetter.Process("owain.test.Tlsvn3.Assigned", sql);
        }

        [Test, Explicit]
        public void Should_send_to_graphite_udp()
        {
            var graphiteUdp = new Graphite.GraphiteUdpClient("metrics.london.ttldev.local");
            //graphiteUdp.Send("Test.Owain.Servers.Udp.One", 55, DateTime.Now);            
            //graphiteUdp.Send("Test.Owain.Servers.two.udp", 33, DateTime.Now);
            //graphiteUdp.Send("Test.Owain.Servers.One.5m", 33, DateTime.Now);
            //graphiteUdp.Send("Test.Owain.Servers.2m.abc", 33, DateTime.Now);
            graphiteUdp.Send("Test.Owain.Servers.Udp.2m", 33, DateTime.Now);
            graphiteUdp.Send("Test.Owain.Servers.Udp.1m", 33, DateTime.Now);
            graphiteUdp.Send("Test.Owain.Servers.Udpp.1m", 33, DateTime.Now);
            graphiteUdp.Send("Test.Owain.Servers.1m.Udp.", 33, DateTime.Now);
            graphiteUdp.Send("Test.Owain.Servers.30s.Udp.", 33, DateTime.Now);
        }

        [Test, Explicit]
        public void Should_try_controller()
        {
            var log = LogManager.GetLogger("log");
            log4net.Config.XmlConfigurator.Configure();
            //var host = "metrics.london.ttldev.local";
            //var port = 2003;
            //var connectionString = "Data Source=nuget.london.ttldev.local;Initial Catalog=GoReportingSvn3;User Id=sa;Password=!bcde1234;";
            //var sqlGetter = new SqlGetter(connectionString, host, port, log);
            //var sql = "SELECT [Assigned],DateDiff(s,[Scheduled],[Assigned]) FROM [GoReportingSvn3].[dbo].[V_BuildStateTransition] where Scheduled > '2012-01-01'";
            //sqlGetter.Process("owain.test.Tlsvn3.Assigned", sql);
            //var ds = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=10.244.127.11)(PORT=1532)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=tracslg2)));User Id=temp_ttl_user;Password=tt1r34dj0b;";
            //var taskParam = new TaskParams("ThePath", "SELECT concat(dm_code,concat('_',current_ds_code)) AS dm, COUNT(DISTINCT (d.ID)) AS dm_count FROM DELIVERIES D WHERE d.tr_ID >= (SELECT TR_ID FROM TRANSACTION_FIRSTOFDAY WHERE date_of_tr = TRUNC(SYSDATE)) GROUP BY dm_code, current_ds_code", ds, "Oracle", "name", "graphiteudp");
            var tasks = new List<IRunTask> { new RunableRunTask(new SqlServerClient(), new DataClientFactory(log, null, null), new GraphiteClientFactory(log), log, new GraphiteTcpClient()) };
            var set = new RunTaskSetWithProcess(tasks, new Stop(), 1000);
            var sleep = new Sleeper();
            var stop = new Stop();
            var controller = new Controller(set, sleep, stop, log);
            controller.Process();
        }
    }
}