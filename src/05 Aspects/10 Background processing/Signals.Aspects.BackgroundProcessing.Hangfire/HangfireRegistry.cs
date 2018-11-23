using Hangfire;
using Hangfire.MemoryStorage;
using Hangfire.Storage;
using Signals.Aspects.BackgroundProcessing.TaskConfiguration;
using Signals.Aspects.BackgroundProcessing.Hangfire.Configuration;
using System;

namespace Signals.Aspects.BackgroundProcessing.Hangfire
{
    /// <summary>
    /// Task repository manager
    /// </summary>
    public class HangfireRegistry : ITaskRegistry
    {
        /// <summary>
        /// Hangfire registry
        /// </summary>
        private readonly BackgroundJobServer _server;

        /// <summary>
        /// CTOR
        /// </summary>
        public HangfireRegistry()
        {            
            GlobalConfiguration.Configuration.UseMemoryStorage();
            _server = new BackgroundJobServer();
        }

        /// <summary>
        /// Schedule task instance
        /// </summary>
        /// <param name="task"></param>
        /// <param name="config"></param>
        public void ScheduleTask(ISyncTask task, SyncTaskConfiguration config)
        {
            _server.Configure(task, config);
        }

	    /// <summary>
	    /// Schedule task by type
	    /// </summary>
	    /// <param name="config"></param>
	    public void ScheduleTask<TTask>(SyncTaskConfiguration config) where TTask : ISyncTask
        {
            var instance = Activator.CreateInstance<TTask>();
            ScheduleTask(instance, config);
        }

        /// <summary>
        /// Start task execution for all scheduled tasks
        /// </summary>
        public void Start()
        {
        }

        /// <summary>
        /// Stop task execution for all scheduled tasks and remove them
        /// </summary>
        public  void Stop()
        {
            _server.SendStop();

            using (var connection = JobStorage.Current.GetConnection())
            {
                foreach (var recurringJob in connection.GetRecurringJobs())
                {
                    RecurringJob.RemoveIfExists(recurringJob.Id);
                }
            }

            _server.Dispose();
        }
    }
}
