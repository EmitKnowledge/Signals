using FluentScheduler;
using Signals.Aspects.BackgroundProcessing.TaskConfiguration;
using Signals.Aspects.BackgroundProcessing.FluentScheduler.Configuration;
using System;
using System.Collections.Generic;

namespace Signals.Aspects.BackgroundProcessing.FluentScheduler
{
    /// <summary>
    /// Task registry using fluent scheduler
    /// </summary>
    public class FluentRegistry : ITaskRegistry
    {
        /// <summary>
        /// FluentScheduler schedules
        /// </summary>
        private readonly List<Schedule> _schedules;

        /// <summary>
        /// CTOR
        /// </summary>
        public FluentRegistry()
        {
            _schedules = new List<Schedule>();
        }

        /// <summary>
        /// Schedule task instance
        /// </summary>
        /// <param name="task"></param>
        /// <param name="config"></param>
        public void ScheduleTask(ISyncTask task, RecurrencePatternConfiguration config)
        {
            _schedules.Configure(task, config);
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
            ScheduleGroup.Start(_schedules);
        }

        /// <summary>
        /// Stop task execution for all scheduled tasks and remove them
        /// </summary>
        public void Stop()
        {
            ScheduleGroup.StopAndBlock(_schedules);
        }
    }
}
