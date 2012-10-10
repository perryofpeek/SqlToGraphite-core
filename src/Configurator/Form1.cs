using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SqlToGraphite;
using SqlToGraphite.Conf;
using SqlToGraphite.Config;

using log4net;

namespace Configurator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private ILog log;

        private ConfigRepository repository;

        private void Form1_Load(object sender, EventArgs e)
        {

            log = LogManager.GetLogger("log");
            log4net.Config.XmlConfigurator.Configure();

            var sleepTime = 1000;
            var assemblyResolver = new AssemblyResolver(new DirectoryImpl());
            var config = new SqlToGraphiteConfig(assemblyResolver);
            var reader =
                new ConfigHttpReader(
                    "file://C:/git/perryOfPeek/SqlToGraphite/src/Configurator/bin/Debugconfig.xml", "", "");
            var cache = new Cache(new TimeSpan(0, 0, 1, 0), log);
            var sleep = new Sleeper();
            var genericSerializer = new GenericSerializer();
            var configPersister = new ConfigPersister(new ConfigFileWriter("somefile.xml"), genericSerializer);

            repository = new ConfigRepository(
                reader, cache, sleep, this.log, sleepTime, configPersister, genericSerializer);

            var clients = repository.GetClients();

        }
    }
}
