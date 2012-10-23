using System;
using System.IO;
using System.Security.Cryptography;

using log4net;

namespace SqlToGraphiteInterfaces
{
    public abstract class PluginBase : Job
    {
        protected IEncryption encryption;

        protected ILog Log { get; set; }

        public override string Name { get; set; }

        public override string ClientName { get; set; }

        public override string Type { get; set; }

        protected PluginBase()
        {
            SetOrCreateLogObjectIfNull();
            this.Type = this.GetType().FullName;
            encryption = new Encryption();
        }

        protected PluginBase(ILog log, Job job, IEncryption encryption)
        {
            this.encryption = encryption;
            this.Log = log;
            this.SetOrCreateLogObjectIfNull();
            this.Name = job.Name;
            this.ClientName = job.ClientName;
            this.Type = job.Type;
        }

        protected string Encrypt(string clearText)
        {
            return encryption.Encrypt(clearText);
        }

        protected string Decrypt(string clearText)
        {
            return encryption.Decrypt(clearText);
        }

        private void SetOrCreateLogObjectIfNull()
        {
            if (this.Log == null)
            {
                this.Log = LogManager.GetLogger("log");
                log4net.Config.XmlConfigurator.Configure();
            }
        }     

        public void WireUpProperties(Job taskParams, object dis)
        {
            var thisPropertyInfos = dis.GetType().GetProperties();
            var test = taskParams.GetType().GetProperties();

            // write property names
            foreach (var propertyInfo in thisPropertyInfos)
            {
                if (null != propertyInfo && propertyInfo.CanWrite)
                {
                    foreach (var info in test)
                    {
                        if (info.Name == propertyInfo.Name)
                        {
                            propertyInfo.SetValue(dis, info.GetValue(taskParams, null), null);
                        }
                    }
                }
            }
        }
    }
}