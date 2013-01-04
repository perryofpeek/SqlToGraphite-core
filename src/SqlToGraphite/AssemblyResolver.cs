using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SqlToGraphite.Config;

namespace SqlToGraphite
{
    using log4net;

    public class AssemblyResolver : IAssemblyResolver
    {
        public const string FilesToScan = "*.dll";

        private readonly IDirectory directory;

        private readonly ILog log;

        public AssemblyResolver(IDirectory directory, ILog log)
        {
            this.directory = directory;
            this.log = log;
            types = new Dictionary<string, Type>();
            this.GetJobTypes(FilesToScan);
            this.GetJobTypes("*.exe");
        }

        private Dictionary<string, Type> types;

        public Type ResolveType(Job job)
        {
            //log.Debug(string.Format("Resolving job of type {0}", job.GetType().FullName));
            if (!this.types.ContainsKey(job.Type))
            {
                throw new PluginNotFoundOrLoadedException(string.Format("The plugin {0} is not found or loaded", job.Type));
            }

            return this.types[job.Type];
        }

        public Type[] ResolveTypes(JobImpl job)
        {
            return this.types.Select(type => type.Value).ToArray();
        }

        public IEnumerable<Type> ResolveAllTypes(JobImpl job)
        {
            return this.types.Select(type => type.Value);
        }

        public Dictionary<string, Type> GetJobTypes(string filesToScan)
        {
            log.Debug(string.Format("Files to scan : {0}", filesToScan));
            var files = directory.GetFilesInCurrentDirectory(filesToScan);
            if (files != null)
            {
                log.Debug(string.Format("Files is not null"));
                foreach (var dll in files)
                {
                    log.Debug(string.Format("Importing File : {0}", dll));
                    this.ImportTypes(dll);
                }
            }

            return types;
        }

        private void ImportTypes(string dll)
        {
            try
            {
                log.Debug(string.Format("Loading {0}", dll));
                this.LoadTypes(Assembly.LoadFile(dll), typeof(Job));
            }
            catch (BadImageFormatException badImageFormatException)
            {
                log.Error(string.Format("Bad image {0} {1}", dll, badImageFormatException.Message));
                log.Error(badImageFormatException);
            }
        }

        private void LoadTypes(Assembly asm, Type typeToLoad)
        {
            //Query our types. We could also load any other assemblies and
            //query them for any types that inherit from Animal
            foreach (Type currType in asm.GetTypes())
            {
                if (!currType.IsAbstract && !currType.IsInterface && typeToLoad.IsAssignableFrom(currType) && currType.IsPublic)
                {
                    if (!types.ContainsKey(currType.FullName))
                    {
                        types.Add(currType.FullName, currType);
                    }
                }
            }
        }

        //TODO: write test(s) for this. 
        public Type ResolveType(string name)
        {
            foreach (var type in types)
            {
                if (type.Key == name)
                {
                    return type.Value;
                }
            }

            return null;
        }
    }
}