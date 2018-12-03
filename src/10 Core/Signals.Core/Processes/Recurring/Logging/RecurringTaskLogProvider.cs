using Signals.Core.Common.Instance;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Signals.Core.Processes.Recurring.Logging
{
    /// <summary>
    /// Sync task logger in-memory implementation
    /// </summary>
    internal class RecurringTaskLogProvider : IRecurringTaskLogProvider
    {
        private static readonly object syncLock = new object();

        /// <summary>
        /// Maximum number of logs
        /// </summary>
        private const int MaxLogs = 100;

        /// <summary>
        /// Logs repository
        /// </summary>
        private static Dictionary<Type, List<RecurringTaskLog>> _recurringTaskLogs;

        /// <summary>
        /// Logs repository provider
        /// </summary>
        private static Dictionary<Type, List<RecurringTaskLog>> RecurringTaskLogs
        {
            get
            {
                if (_recurringTaskLogs.IsNull()) _recurringTaskLogs = new Dictionary<Type, List<RecurringTaskLog>>();
                return _recurringTaskLogs;
            }
        }

        static RecurringTaskLogProvider()
        {
            _recurringTaskLogs = new Dictionary<Type, List<RecurringTaskLog>>();
        }

        /// <summary>
        /// Insert sync task result
        /// </summary>
        /// <param name="log"></param>
        public void CreateLog(RecurringTaskLog log)
        {
            lock (syncLock)
            {
                if (!RecurringTaskLogs.ContainsKey(log.ProcessType))
                {
                    RecurringTaskLogs.Add(log.ProcessType, new List<RecurringTaskLog>());
                }
                var list = RecurringTaskLogs[log.ProcessType];
                log.Id = list.Count + 1;

                list.Add(log);
                RecurringTaskLogs[log.ProcessType] = RecurringTaskLogs[log.ProcessType]
                                                    .OrderByDescending(x => x.StartTime)
                                                    .Take(MaxLogs)
                                                    .ToList();
            }
        }

        /// <summary>
        /// Get current sync execution result if it is not finished
        /// </summary>
        /// <param name="processType"></param>
        /// <returns></returns>
        public RecurringTaskLog Current(Type processType)
        {
            lock (syncLock)
            {
                if (!RecurringTaskLogs.ContainsKey(processType))
                {
                    RecurringTaskLogs.Add(processType, new List<RecurringTaskLog>());
                }

                return RecurringTaskLogs[processType].SingleOrDefault(x => !x.EndTime.HasValue);
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
            lock (syncLock)
            {
                if (!RecurringTaskLogs.ContainsKey(processType))
                {
                    RecurringTaskLogs.Add(processType, new List<RecurringTaskLog>());
                }

                return RecurringTaskLogs[processType]
                        .OrderByDescending(x => x.StartTime)
                        .Take(take)
                        .ToList();
            }
        }

        /// <summary>
        /// Get last sync execution result
        /// </summary>
        /// <param name="processType"></param>
        /// <returns></returns>
        public RecurringTaskLog Last(Type processType)
        {
            lock (syncLock)
            {
                if (!RecurringTaskLogs.ContainsKey(processType))
                {
                    RecurringTaskLogs.Add(processType, new List<RecurringTaskLog>());
                }

                return RecurringTaskLogs[processType].OrderByDescending(x => x.StartTime).FirstOrDefault();
            }
        }

        /// <summary>
        /// Update sync task result
        /// </summary>
        /// <param name="log"></param>
        public void UpdateLog(RecurringTaskLog log)
        {
            lock (syncLock)
            {
                if (!RecurringTaskLogs.ContainsKey(log.ProcessType))
                {
                    RecurringTaskLogs.Add(log.ProcessType, new List<RecurringTaskLog>());
                }

                var existing = RecurringTaskLogs[log.ProcessType].SingleOrDefault(x => x.Id == log.Id);
                if (!existing.IsNull())
                {
                    existing.Result = log.Result;
                    existing.EndTime = log.EndTime;
                    existing.IsFaulted = log.IsFaulted;
                }
            }
        }
    }
}
