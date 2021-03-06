﻿using System.Collections.Generic;
using System.IO;

using SqlToGraphite.Config;

namespace SqlToGraphite.Conf
{
    public class ConfigPersister : IConfigPersister
    {
        private readonly IConfigWriter configWriter;

        private readonly IGenericSerializer genericSerializer;

        public ConfigPersister(IConfigWriter configWriter, IGenericSerializer genericSerializer)
        {
            this.configWriter = configWriter;
            this.genericSerializer = genericSerializer;
        }

        public void Save(SqlToGraphiteConfig config)
        {
            var data = genericSerializer.Serialize(config);
            configWriter.Save(data);
        }

        public void Save(SqlToGraphiteConfig config, string path)
        {            
            var data = genericSerializer.Serialize(config);
            configWriter.Save(data, path);
        }
    }
}