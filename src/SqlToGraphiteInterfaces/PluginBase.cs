using System;
using System.Collections.Generic;
using System.Xml.Serialization;

using SqlToGraphite;

using log4net;

namespace SqlToGraphiteInterfaces
{
    public abstract class PluginBase : Job
    {
        protected ILog Log { get; set; }

        protected Job TaskParams { get; set; }

        public override string Name { get; set; }

        public override string ClientName { get; set; }

        public override string Type { get; set; }

        //[XmlIgnore]
        //private Type t;

        //[XmlIgnore]
        //public Type T
        //{
        //    get
        //    {
        //        return t;
        //    }
        //    set
        //    {
        //        t = value;
        //        this.TName = value.AssemblyQualifiedName;
        //    }
        //}

        //public string TName
        //{
        //    get { return t.AssemblyQualifiedName; }
        //    set { t = Type.GetType(); }
        //}

        protected PluginBase(ILog log, Job taskParams)
        {
            this.Log = log;
            this.TaskParams = taskParams;
            this.Name = taskParams.Name;
            this.ClientName = taskParams.ClientName;
            this.Type = taskParams.Type;
        }

        protected PluginBase()
        {
            this.Type = this.GetType().FullName;
        }

        //public abstract IList<IResult> Get();
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