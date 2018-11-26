using FluentScheduler;
using Signals.Aspects.BackgroundProcessing.TaskConfiguration;
using Signals.Aspects.BackgroundProcessing.FluentScheduler.Configuration;
using System;

namespace Signals.Aspects.BackgroundProcessing.FluentScheduler
{
    public class FluentRegistry : ITaskRegistry
    {
        /// <summary>
        /// FluentScheduler registry
        /// </summary>
        private readonly Registry _registry;

        /// <summary>
        /// CTOR
        /// </summary>
        public FluentRegistry()
        {
            _registry = new Registry();
        }

        /// <summary>
        /// Schedule task instance
        /// </summary>
        /// <param name="task"></param>
        /// <param name="config"></param>
        public void ScheduleTask(ISyncTask task, RecurrencePatternConfiguration config)
        {
            _registry.Configure(task, config);
        }

	    /// <summary>
	    /// Schedule task by type
	    /// </summary>
	    /// <param name="config"></param>
	    public void ScheduleTask<TTask>(RecurrencePatternConfiguration config) where TTask : ISyncTask
        {
            var instance = Activator.CreateInstance<TTask>();
            ScheduleTask(instance, config);
        }

        /// <summary>
        /// Start task execution for all scheduled tasks
        /// </summary>
        public void Start()
        {
            JobManager.Initialize(_registry);
        }

        /// <summary>
        /// Stop task execution for all scheduled tasks and remove them
        /// </summary>
        public void Stop()
        {
            JobManager.StopAndBlock();
            JobManager.RemoveAllJobs();
        }
    }
}
