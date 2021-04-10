using Signals.Aspects.BackgroundProcessing.TaskConfiguration;

namespace Signals.Aspects.BackgroundProcessing
{
    /// <summary>
    /// Task repository manager
    /// </summary>
    public interface ITaskRegistry
    {
        /// <summary>
        /// Schedule task instance
        /// </summary>
        /// <param name="task"></param>
        /// <param name="config"></param>
        void ScheduleTask(ISyncTask task, RecurrencePatternConfiguration config);

        /// <summary>
        /// Schedule task by type
        /// </summary>
        /// <typeparam name="TTask"></typeparam>
        /// <param name="config"></param>
        void ScheduleTask<TTask>(RecurrencePatternConfiguration config) where TTask : ISyncTask;

        /// <summary>
        /// Start task execution for all scheduled tasks
        /// </summary>
        void Start();

        /// <summary>
        /// Stop task execution for all scheduled tasks and remove them
        /// </summary>
        void Stop();
    }
}
