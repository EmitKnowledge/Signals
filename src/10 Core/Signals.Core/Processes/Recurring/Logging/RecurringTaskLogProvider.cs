using Signals.Core.Common.Instance;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Signals.Core.Common.Serialization;

namespace Signals.Core.Processes.Recurring.Logging
{
    /// <summary>
    /// Sync task logger in-memory implementation
    /// </summary>
    internal class RecurringTaskLogProvider : IRecurringTaskLogProvider
    {
        /// <summary>
        /// Synchronization lock
        /// </summary>
        private static readonly object SyncLock = new object();
        
        /// <summary>
        /// Maximum number of logs
        /// </summary>
        private const int MaxLogs = 1000;

        /// <summary>
        /// Logs repository provider
        /// </summary>
        private static readonly ConcurrentDictionary<Type, List<RecurringTaskLog>> RecurringTaskLogs;

        /// <summary>
        /// CTOR
        /// </summary>
        static RecurringTaskLogProvider()
        {
            RecurringTaskLogs = new ConcurrentDictionary<Type, List<RecurringTaskLog>>();
            typeof(RecurringTaskLogProvider).D($"Initializing RecurringTaskLogProvider. Max logs: {MaxLogs}.");
        }

        /// <summary>
        /// Insert sync task result
        /// </summary>
        /// <param name="log"></param>
        public void CreateLog(RecurringTaskLog log)
        {
            lock (SyncLock)
            {
                if (!RecurringTaskLogs.ContainsKey(log.ProcessType))
                {
                    RecurringTaskLogs.TryAdd(log.ProcessType, new List<RecurringTaskLog>());
                }

                RecurringTaskLogs[log.ProcessType].Add(log);
                RecurringTaskLogs[log.ProcessType] = RecurringTaskLogs[log.ProcessType]
                                                        .OrderByDescending(x => x.StartTime)
                                                        .Take(MaxLogs)
                                                        .ToList();
                this.D($"Creating log for: {log?.ProcessType?.FullName} -> Log metadata: {log?.SerializeJson()}.");
            }
        }

        /// <summary>
        /// Get current sync execution result if it is not finished
        /// </summary>
        /// <param name="processType"></param>
        /// <returns></returns>
        public RecurringTaskLog Current(Type processType)
        {
            lock (SyncLock)
            {
                if (!RecurringTaskLogs.ContainsKey(processType))
                {
                    RecurringTaskLogs.TryAdd(processType, new List<RecurringTaskLog>());
                }

                var lastRecurringTaskLog = RecurringTaskLogs[processType].FirstOrDefault(x => !x.EndTime.HasValue);
                this.D($"Returning current log for: {processType?.FullName} -> Log metadata: {lastRecurringTaskLog?.SerializeJson()}.");
                return lastRecurringTaskLog;
            }
        }

        /// <summary>
        /// Get last sync execution results
        /// </summary>
        /// <param name="processType"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public List<RecurringTaskLog> Last(Type processType, int take)
        {
            lock (SyncLock)
            {
                if (!RecurringTaskLogs.ContainsKey(processType))
                {
                    RecurringTaskLogs.TryAdd(processType, new List<RecurringTaskLog>());
                }

                var recurringTaskLogs = RecurringTaskLogs[processType]
                        .OrderByDescending(x => x.StartTime)
                        .Take(take)
                        .ToList();

                this.D($"Returning last {take} logs for: {processType?.FullName} -> Total logs: {recurringTaskLogs.Count}.");

                return recurringTaskLogs;
            }
        }

        /// <summary>
        /// Get last sync execution result
        /// </summary>
        /// <param name="processType"></param>
        /// <returns></returns>
        public RecurringTaskLog Last(Type processType)
        {
            lock (SyncLock)
            {
                if (!RecurringTaskLogs.ContainsKey(processType))
                {
                    RecurringTaskLogs.TryAdd(processType, new List<RecurringTaskLog>());
                }

                var lastRecurringTaskLog = RecurringTaskLogs[processType]
						.OrderByDescending(x => x.StartTime)
						.FirstOrDefault();

                this.D($"Returning last log for: {processType?.FullName} -> Log metadata: {lastRecurringTaskLog?.SerializeJson()}.");

                return lastRecurringTaskLog;
            }
        }

        /// <summary>
        /// Update sync task result
        /// </summary>
        /// <param name="log"></param>
        public void UpdateLog(RecurringTaskLog log)
        {
        }
    }
}
