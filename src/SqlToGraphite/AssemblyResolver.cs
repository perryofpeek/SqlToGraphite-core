using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SqlToGraphite.Config;

namespace SqlToGraphite
{
    using System.Xml.Serialization;

    public class AssemblyResolver : IAssemblyResolver
    {
        public const string FilesToScan = "*.dll";

        private readonly IDirectory directory;

        public AssemblyResolver(IDirectory directory)
        {
            this.directory = directory;
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
            var files = directory.GetFilesInCurrentDirectory(filesToScan);           
            if (files != null)
            {
                foreach (var dll in files)
                {
                    this.ImportTypes(dll);
                }
            }

            return types;
        }

        private void ImportTypes(string dll)
        {
            try
            {
                this.LoadTypes(Assembly.LoadFile(dll), typeof(Job));
            }
            catch (BadImageFormatException badImageFormat)
            {
                var s = "a";
                var y = badImageFormat.Message;
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