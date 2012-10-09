using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using log4net;

namespace SqlToGraphiteInterfaces
{
    public abstract class PluginBase : Job
    {
        protected ILog Log { get; set; }

       // protected Job TaskParams { get; set; }

        public override string Name { get; set; }

        public override string ClientName { get; set; }

        public override string Type { get; set; }

        protected PluginBase(ILog log, Job job)
        {
            this.Log = log;
            this.Name = job.Name;
            this.ClientName = job.ClientName;
            this.Type = job.Type;
        }

        protected PluginBase()
        {
            this.Type = this.GetType().FullName;
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