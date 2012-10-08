using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using log4net;
using SqlToGraphiteInterfaces;

namespace SqlToGraphite
{
    public class DataClientFactory : IDataClientFactory
    {
        private readonly ILog log;

        private List<Type> types;

        public DataClientFactory(ILog log)
        {
            this.log = log;
            types = GetJobTypes();
        }

        public static List<Type> GetJobTypes()
        {
            var types = new List<Type>();

            var allDlls = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.dll");
            foreach (var dll in allDlls)
            {
                try
                {
                    Assembly a = Assembly.LoadFile(dll);
                    types.AddRange(LoadJobTypes(a).ToArray());
                }
                catch (Exception ex)
                {
                    var s = ex.Message;
                    throw;
                }
            }

            return types;
        }

        private static List<Type> LoadJobTypes(Assembly asm)
        {
            var types = new List<Type>();
            var job = typeof(Job);

            //Query our types. We could also load any other assemblies and
            //query them for any types that inherit from Animal
            foreach (Type currType in asm.GetTypes())
            {
                if (!currType.IsAbstract && !currType.IsInterface && job.IsAssignableFrom(currType))
                {
                    types.Add(currType);
                }
            }

            return types;
        }

        public ISqlClient Create(Job job)
        {            
            Type t1 = this.ResolveJobType(job);

            if (t1 == null)
            {
                throw new UnknownDataClientException(job.Type);
            }

            Foo(t1, new object[] { log, job });
            var obj = Activator.CreateInstance(t1, new object[] { log, job });
            return (ISqlClient)obj;
        }

        private Type ResolveJobType(Job job)
        {
            Type t1 = null;
            foreach (var type in this.types)
            {
                if (type.FullName == job.Type)
                {
                    t1 = type;
                    break;
                }
            }

            return t1;
        }

        public static void Foo(Type t, params object[] p)
        {
            System.Diagnostics.Debug.WriteLine("<---- foo");
            foreach (System.Reflection.ConstructorInfo ci in t.GetConstructors())
            {
                System.Diagnostics.Debug.WriteLine(t.FullName + ci.ToString());
            }

            foreach (object o in p)
            {
                System.Diagnostics.Debug.WriteLine("param:" + o.GetType().FullName);
            }

            System.Diagnostics.Debug.WriteLine("foo ---->");
        }
    }
}