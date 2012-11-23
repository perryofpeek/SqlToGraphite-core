using System;
using System.Collections.Generic;
using System.Collections.Specialized;

using Interop.MSUtil;

using SqlToGraphiteInterfaces;
using log4net;

using LogQuery = Interop.MSUtil.LogQueryClassClass;
using EventLogInputFormat = Interop.MSUtil.COMEventLogInputContextClassClass;
using LogRecordSet = Interop.MSUtil.ILogRecordset;

namespace SqlToGraphite.Plugin.LogParser
{
    public class LogParserClient : PluginBase
    {
        public string Path { get; set; }

        public LogParserClient()
        {
        }

        public LogParserClient(ILog log, Job job, IEncryption encryption)
            : base(log, job, encryption)
        {
            this.WireUpProperties(job, this);
        }

        private Result Map(ILogRecord record)
        {
            var value = -1;
            var dateTime = DateTime.Now;
            var name = string.Empty;

            var r = record.getValue(0);
            if ((r is string))
            {
                name = Convert.ToString(r);
            }

            if ((r is int) || (r is decimal))
            {
                value = Convert.ToInt32(r);
            }

            r = record.getValue(1);
            if ((r is string))
            {
                name = Convert.ToString(r);
            }

            if ((r is int) || (r is decimal))
            {
                value = Convert.ToInt32(r);
            }

            //Console.WriteLine(record.getValue(0).GetType());
            //Console.WriteLine(record.getValue(1).GetType());
            try
            {
                //Console.WriteLine(record.getValue(2));
            }
            catch (Exception ex)
            {

                var s = ex.Message;
            }

            //Console.WriteLine(string.Format("{0} {1}", record.getValue(0), record.getValue(1)));


            //for (int i = 0; i < record.; i++)
            //{
            //    if (record[i] is string)
            //    {
            //        name = Convert.ToString(record[i]);
            //    }

            //    if (record[i] is int || record[i] is decimal)
            //    {
            //        value = Convert.ToInt32(record[i]);
            //    }

            //    if (record[i] is DateTime)
            //    {
            //        dateTime = Convert.ToDateTime(record[i]);
            //    }
            //}

            if (this.Name != string.Empty && name == string.Empty)
            {
                name = this.Name;
            }

            this.Log.Debug(string.Format("Got [{1}] {0}", value, dateTime));
            var result = new Result(name, dateTime, this.Path);
            result.SetValue(value);
            return result;
        }

        public override string Name { get; set; }

        public override string ClientName { get; set; }

        public override string Type { get; set; }

        public override IList<IResult> Get()
        {
            var rtn = new List<IResult>();
            const string CheckpointFile = @"checkpoint.lpc";

            var logParser = new LogQueryClassClass();
            var w3Clog = new COMIISW3CInputContextClass
            {
                iCheckpoint = CheckpointFile
            };

            var records = logParser.Execute(this.Sql, w3Clog);
            while (!records.atEnd())
            {
                var record = records.getRecord();
                rtn.Add(this.Map(record));
                records.moveNext();
            }
            return rtn;
        }

        public string Sql { get; set; }

        public string PathName { get; set; }

        private static void Parse(string checkpointFile, string strSQL)
        {
            var logParser = new LogQueryClassClass();
            var w3Clog = new COMIISW3CInputContextClass
            {
                iCheckpoint = checkpointFile
            };
            var logRecordset = logParser.Execute(strSQL, w3Clog);
            while (!logRecordset.atEnd())
            {
                var logRecord = logRecordset.getRecord();
                //Console.WriteLine(logRecord.getValue(0).GetType());
                //Console.WriteLine(logRecord.getValue(1).GetType());
                //Console.WriteLine(string.Format("{0} {1}", logRecord.getValue(0), logRecord.getValue(1)));
                logRecordset.moveNext();
            }
        }
    }
}
