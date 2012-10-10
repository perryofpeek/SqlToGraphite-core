using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SqlToGraphite.Config;

namespace SqlToGraphite
{
    public class AssemblyResolver : IAssemblyResolver
    {
        public const string FilesToScan = "*.dll";        

        private readonly IDirectory directory;

        public AssemblyResolver(IDirectory directory)
        {            
            this.directory = directory;
            types = new Dictionary<string, Type>();
            this.GetJobTypes();
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

        public Dictionary<string, Type> GetJobTypes()
        {
            var files = directory.GetFilesInCurrentDirectory(FilesToScan);
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
            catch (BadImageFormatException)
            {
                //this.log.Error(string.Format("Ignore bad image format exception {0}", dll));
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
                    types.Add(currType.FullName, currType);
                }
            }
        }
    }
}