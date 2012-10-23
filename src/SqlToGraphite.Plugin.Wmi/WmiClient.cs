using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using log4net;
using SqlToGraphiteInterfaces;

namespace SqlToGraphite.Plugin.Wmi
{
    public class WmiClient : PluginBase
    {
        private readonly string machineName;

        public string Username { get; set; }

        public string Hostname { get; set; }

        public const string RootPath = @"root\CIMV2";

        public string Path { get; set; }

        public string Sql { get; set; }

        public WmiClient()
        {

        }

        public WmiClient(ILog log, Job job, IEncryption encryption)
            : base(log, job, encryption)
        {
            this.machineName = Environment.MachineName; ;
            this.WireUpProperties(job, this);
        }

        public override string Name { get; set; }

        public override string ClientName { get; set; }

        public override string Type { get; set; }

        public string Password { get; set; }

        public override IList<IResult> Get()
        {
            var rtn = new List<IResult>();

            try
            {
                foreach (ManagementObject o in this.GetWmiObject(this.Sql, this.machineName, RootPath))
                {
                    var value = -1;
                    var dateTime = DateTime.Now;
                    var name = string.Empty;

                    foreach (var col in o.Properties)
                    {
                        if (col.Type == CimType.String)
                        {
                            name = Convert.ToString(col.Value);
                        }

                        if (col.Type == CimType.UInt32)
                        {
                            value = Convert.ToInt32(col.Value);
                        }

                        if (col.Type == CimType.UInt64)
                        {
                            value = Convert.ToInt32(col.Value);
                        }

                        if (col.Type == CimType.DateTime)
                        {
                            dateTime = Convert.ToDateTime(col.Value);
                        }
                    }

                    this.Log.Debug(string.Format("Name {0} value {1} datetime {2}", name, value, dateTime));
                    rtn.Add(new Result(value, name, dateTime, this.Path));
                }
            }
            catch (ManagementException e)
            {
                this.Log.Error(string.Format("Error with {0} {1} {2}", this.Type, this.Path, this.Sql));
                this.Log.Error(e.Message);
                this.Log.Error(e.StackTrace);
            }

            return rtn;
        }

        private static string GetTheNameOfTheColumn(ManagementBaseObject queryObj)
        {
            var name = string.Empty;
            foreach (var v in queryObj.Properties)
            {
                name = v.Name;
            }

            return name;
        }

        private IEnumerable<ManagementObject> GetWmiObject(string query, string machineName, string rootPath)
        {
            try
            {
                var conn = new ConnectionOptions();
                var path = string.Format(@"\\{0}\{1}", machineName, rootPath);
                if (!string.IsNullOrEmpty(this.Username))
                {
                    conn.Username = this.Username;
                }
                if (!string.IsNullOrEmpty(this.Password))
                {
                    conn.Password = this.Password;
                }

                path = string.Format(@"\\{0}\{1}", Hostname, rootPath);
                var scope = new ManagementScope(path, conn);
                this.Log.Debug(string.Format("{0} {1}", path, query));
                var queryObject = new ObjectQuery(query);
                var searcher = new ManagementObjectSearcher(scope, queryObject);
                return searcher.Get().Cast<ManagementObject>().ToList();
            }
            catch (Exception e)
            {
                this.Log.Debug(e);
                throw;
            }
        }

        public static int ConvertToInt(string value)
        {
            var l = Convert.ToInt64(value);
            if (l > Int16.MaxValue)
            {
                l = l / 1024;
            }

            return Convert.ToInt32(l);
        }
    }
}