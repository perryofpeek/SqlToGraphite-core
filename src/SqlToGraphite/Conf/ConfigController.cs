using System;
using System.Collections.Generic;
using System.Linq;

using log4net;

namespace SqlToGraphite.Conf
{
    public class ConfigController : IConfigController
    {
        private readonly IConfigMapper configMapper;

        private readonly ILog log;

        private readonly IConfigRepository configRepository;

        private readonly IRoleConfigFactory roleConfigFactory;

        private readonly IEnvironment environment;

        private readonly ITaskSetBuilder taskSetBuilder;

        private bool newConfig;

        public ConfigController(IConfigMapper configMapper, ILog log, IConfigRepository configRepository, IRoleConfigFactory roleConfigFactory, IEnvironment environment, ITaskSetBuilder taskSetBuilder)
        {
            this.configMapper = configMapper;
            this.log = log;
            this.configRepository = configRepository;
            this.roleConfigFactory = roleConfigFactory;
            this.environment = environment;
            this.taskSetBuilder = taskSetBuilder;
        }

        private IList<IRunTaskSet> currentTaskSet;

        public IList<IRunTaskSet> GetTaskList(string path)
        {
           newConfig = false;
            configRepository.Load();
            if (configRepository.IsNewConfig)
            {
                log.Debug("New configuration");
                if (configRepository.Validate())
                {
                    log.Debug("configuration validated");
                    try
                    {
                        newConfig = true;
                        var roleConfig = roleConfigFactory.Create(configRepository, environment);
                        var templates = configRepository.GetTemplates();
                        var setList = taskSetBuilder.BuildTaskSet(templates, roleConfig);
                        currentTaskSet = configMapper.Map(setList);                   
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);                        
                    }
                }
            }

            return currentTaskSet;
        }

        public IList<IThread> GetTaskThreads(string path)
        {
            return GetTaskList(path).Select(taskSet => new ThreadImpl(taskSet.Process)).Cast<IThread>().ToList();
        }

        public ITaskBag GetTaskBag(string path)
        {
            return new TaskBag(this.GetTaskThreads(path));
        }

        public bool IsNewConfig()
        {
            return newConfig;
        }
    }
}