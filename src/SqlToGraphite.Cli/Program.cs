using System.IO;

using SqlToGraphite.Clients;

using log4net;

namespace SqlToGraphite.Cli
{
    class Program
    {
        private static ILog log;

        static void Main(string[] args)
        {
            //CreateLogger();
            //IDataClientFactory dataClientFactory = new DataClientFactory(log);
            //IGraphiteClientFactory graphiteClientFactory = new GraphiteClientFactory(log);
            //var sr = new StreamReader("config.xml");
            //var config = new Config(sr, new KnownGraphiteClients());
            //var configMapper = new ConfigMapper("hostname", new Stop(), dataClientFactory, graphiteClientFactory, log);
            //var configs = configMapper.Map(config);
            //var taskManager = new TaskManager(log, null, "", new Stop(), null, 1000);
            //taskManager.Start();
        }

        private static void CreateLogger()
        {
            log = LogManager.GetLogger("log");
            log4net.Config.XmlConfigurator.Configure();
        }
    }
}
