using System.Collections.Generic;

namespace SqlToGraphite.Conf
{
    public interface IConfigController
    {
        IList<ITaskSet> GetTaskList(string path);

        IList<IThread> GetTaskThreads(string path);

        ITaskBag GetTaskBag(string path);

        bool IsNewConfig();
    }
}