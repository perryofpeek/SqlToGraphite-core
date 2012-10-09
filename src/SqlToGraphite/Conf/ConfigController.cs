using System;
using System.Collections.Generic;
using System.Linq;

using ConfigSpike.Config;

using log4net;

namespace SqlToGraphite.Conf
{
    public class ConfigController : IConfigController
    {
        private readonly IConfigMapper configMapper;

        private readonly ILog log;

        private readonly IConfigRepository configRepository;

        public ConfigController(IConfigMapper configMapper, ILog log, IConfigRepository configRepository)
        {
            this.configMapper = configMapper;
            this.log = log;
            this.configRepository = configRepository;
        }

        public IList<IRunTaskSet> GetTaskList(string path)
        {
            newConfig = false;
            configRepository.Load();
            if (configRepository.Validate())
            {
                newConfig = true;
                var roleConfig = new RoleConfig(this.configRepository.GetHosts(), Environment.MachineName);
                var templates = configRepository.GetTemplates();
                var roleList = roleConfig.GetRoleListToRunOnThisMachine();

                var setList = new List<TaskSet>();
                foreach (var template in templates)
                {
                    foreach (var wi in template.WorkItems)
                    {
                        if (roleList.Contains(wi.RoleName))
                        {
                            setList.AddRange(wi.TaskSet.ToArray());
                        }
                    }
                }

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