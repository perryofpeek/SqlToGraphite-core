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

        public ConfigController(IConfigMapper configMapper, ILog log, IConfigRepository configRepository, IRoleConfigFactory roleConfigFactory, IEnvironment environment, ITaskSetBuilder taskSetBuilder)
        {
            this.configMapper = configMapper;
            this.log = log;
            this.configRepository = configRepository;
            this.roleConfigFactory = roleConfigFactory;
            this.environment = environment;
            this.taskSetBuilder = taskSetBuilder;
        }

        public IList<IRunTaskSet> GetTaskList(string path)
        {
            newConfig = false;
            configRepository.Load();
            if (configRepository.Validate())
            {
                newConfig = true;
                var roleConfig = roleConfigFactory.Create(configRepository, environment);
                var templates = configRepository.GetTemplates();
                var setList = taskSetBuilder.BuildTaskSet(templates, roleConfig);
                var taskList = configMapper.Map(setList);
                return taskList;
            }

            return null;
        }

        public IList<IThread> GetTaskThreads(string path)
        {
            return GetTaskList(path).Select(taskSet => new ThreadImpl(taskSet.Process)).Cast<IThread>().ToList();
        }

        public ITaskBag GetTaskBag(string path)
        {
            return new TaskBag(this.GetTaskThreads(path));
        }

        private bool newConfig;

        public bool IsNewConfig()
        {
            return newConfig;
        }
    }
}