using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using log4net;
using NUnit.Framework;
using Rhino.Mocks;
using SqlToGraphiteInterfaces;

namespace SqlToGraphite.UnitTests
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class With_AssemblyLoader
    {
        [Test]
        public void Should_()
        {
            var dir = Directory.GetCurrentDirectory();            
            var p = GetPlugins<ISqlClient>(dir);
            foreach (var item in p)
            {
                Console.WriteLine(item.GetType());
            }

            Assert.That(p.Count, Is.EqualTo(3));
        }

        public List<T> GetPlugins<T>(string folder)
        {
            string[] files = Directory.GetFiles(folder, "*.dll");
            var plugins = new List<T>();
            foreach (string file in files)
            {               
                try
                {
                    Assembly assembly = Assembly.LoadFile(file);
                    foreach (Type type in assembly.GetTypes())
                    {
                        if (!type.IsClass || type.IsNotPublic)
                        {
                            continue;
                        }

                        ILog log = LogManager.GetLogger("log");
                        var taskParams = new TaskParams("p", "s", "cs", "t", "n", "c");
                        var interfaces = type.GetInterfaces();                        
                        if (((IList)interfaces).Contains(typeof(T)))
                        {
                            var obj = Activator.CreateInstance(type, new object[] { log, taskParams });
                            var t = (T)obj;
                            plugins.Add(t);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

            return plugins;
        }
    }
}