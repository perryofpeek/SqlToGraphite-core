using System;
using System.IO;

using Interop.MSUtil;

using Rhino.Mocks;

using SqlToGraphiteInterfaces;

using log4net;

using LogQuery = Interop.MSUtil.LogQueryClassClass;
using EventLogInputFormat = Interop.MSUtil.COMEventLogInputContextClassClass;
using LogRecordSet = Interop.MSUtil.ILogRecordset;


using NUnit.Framework;

namespace SqlToGraphite.Plugin.LogParser.UnitTests
{
    [TestFixture]
    // ReSharper disable InconsistentNaming
    public class With_LogParserClient
    {
        [SetUp]
        public void SetUp()
        {

        }

        [Test]
        public void Should_parse_log_file()
        {
            const string checkpointFile = @"checkpoint.lpc";
            const string inputFile = @"test-iis.log";

            DeleteFile(checkpointFile);
            File.Copy("TestLogFile.txt", inputFile, true);

            var logMock = MockRepository.GenerateMock<ILog>();
            string path = "Some.path";
            var pathName = "Test";
            string sql = string.Format(@"select '{1}' , count (*) from {0} WHERE cs-uri-stem LIKE '%/buytickets/Default.aspx%' group by cs-uri-stem ", "TestLogFile.txt", pathName);            
            var job = new LogParserClient();
            job.Sql = sql;
            job.Path = path;
            job.PathName = pathName;
            IEncryption encryption = MockRepository.GenerateMock<IEncryption>(); 
            var logParserClient = new LogParserClient(logMock, job, encryption);
            //Test
            var result = logParserClient.Get();
            //Assert
            Assert.That(result[0].Value, Is.EqualTo(1000));
            Assert.That(result[0].FullPath, Is.EqualTo(string.Format("{0}.{1}", path, pathName)));
            Assert.That(result[0].Name, Is.EqualTo(pathName));
        }


        [Test]
        public void Should_too()
        {

            const string checkpointFile = @"checkpoint.lpc";
            const string inputFile = @"test-iis.log";

            DeleteFile(checkpointFile);
            DeleteFile(inputFile);
            File.Copy("TestLogFile.txt", inputFile, true);
            //string strSQL1 = string.Format(@"select distinct sc-status , count (*) from {0} group by sc-status", "TestLogFile.txt");
            //string strSQL2 = string.Format(@"select distinct sc-status , count (*) from {0} group by sc-status", "TestLogFile2.txt");

            string strSQL1 = string.Format(@"select cs-uri-stem , count (*) from {0} WHERE cs-uri-stem LIKE '%/buytickets/portals/sme/images/logo.gif%' group by cs-uri-stem ", "TestLogFile.txt");
            //string strSQL2 = string.Format(@"select distinct sc-status , count (*) from {0} group by sc-status", "TestLogFile2.txt");



            Parse(checkpointFile, strSQL1);
            //Parse(checkpointFile, strSQL2);
        }

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

        private static void DeleteFile(string checkpointFile)
        {
            if (File.Exists(checkpointFile))
            {
                File.Delete(checkpointFile);
            }
        }

        [Test]
        public void Should_Parse_Logs()
        {
            try
            {
                // Instantiate the LogQuery object
                LogQuery oLogQuery = new LogQuery();

                // Instantiate the Event Log Input Format object
                EventLogInputFormat oEVTInputFormat = new EventLogInputFormat();

                // Set its "direction" parameter to "BW"
                oEVTInputFormat.direction = "BW";

                // Create the query
                string query = @"SELECT TOP 50 SourceName, EventID, Message FROM System";

                // Execute the query
                LogRecordSet oRecordSet = oLogQuery.Execute(query, oEVTInputFormat);

                // Browse the recordset
                for (; !oRecordSet.atEnd(); oRecordSet.moveNext())
                {
                    //Console.WriteLine(oRecordSet.getRecord().toNativeString(","));
                }

                // Close the recordset
                oRecordSet.close();
            }
            catch (System.Runtime.InteropServices.COMException exc)
            {
                Console.WriteLine("Unexpected error: " + exc.Message);
            }

        }
    }
}
