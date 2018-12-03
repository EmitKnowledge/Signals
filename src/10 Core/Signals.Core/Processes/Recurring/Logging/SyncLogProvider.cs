using Signals.Core.Common.Instance;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Signals.Core.Processes.Recurring.Logging
{
    /// <summary>
    /// Sync task logger in-memory implementation
    /// </summary>
    internal class SyncLogProvider : ISyncLogProvider
    {
        /// <summary>
        /// Maximum number of logs
        /// </summary>
        private const int MaxLogs = 100;

        /// <summary>
        /// Logs repository
        /// </summary>
        private static Dictionary<Type, List<SyncTaskLog>> _syncTaskLogs;

        /// <summary>
        /// Logs repository provider
        /// </summary>
        private static Dictionary<Type, List<SyncTaskLog>> SyncTaskLogs
        {
            get
            {
                if (_syncTaskLogs.IsNull()) _syncTaskLogs = new Dictionary<Type, List<SyncTaskLog>>();
                return _syncTaskLogs;
            }
        }

        static SyncLogProvider()
        {
            _syncTaskLogs = new Dictionary<Type, List<SyncTaskLog>>();
        }

        /// <summary>
        /// Insert sync task result
        /// </summary>
        /// <param name="log"></param>
        public void CreateLog(SyncTaskLog log)
        {
            lock (_syncTaskLogs)
            {
                if (!SyncTaskLogs.ContainsKey(log.ProcessType))
                {
                    SyncTaskLogs.Add(log.ProcessType, new List<SyncTaskLog>());
                }
                var list = SyncTaskLogs[log.ProcessType];
                log.Id = list.Count + 1;

                list.Add(log);
                SyncTaskLogs[log.ProcessType] = SyncTaskLogs[log.ProcessType]
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
        public SyncTaskLog Current(Type processType)
        {
            lock (_syncTaskLogs)
            {
                if (!SyncTaskLogs.ContainsKey(processType))
                {
                    SyncTaskLogs.Add(processType, new List<SyncTaskLog>());
                }

                return SyncTaskLogs[processType].SingleOrDefault(x => !x.EndTime.HasValue);
            }
        }

        /// <summary>
        /// Get last sync execution results
        /// </summary>
        /// <param name="processType"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public List<SyncTaskLog> Last(Type processType, int take)
        {
            lock (_syncTaskLogs)
            {
                if (!SyncTaskLogs.ContainsKey(processType))
                {
                    SyncTaskLogs.Add(processType, new List<SyncTaskLog>());
                }

                return SyncTaskLogs[processType]
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
        public SyncTaskLog Last(Type processType)
        {
            lock (_syncTaskLogs)
            {
                if (!SyncTaskLogs.ContainsKey(processType))
                {
                    SyncTaskLogs.Add(processType, new List<SyncTaskLog>());
                }

                return SyncTaskLogs[processType].OrderByDescending(x => x.StartTime).FirstOrDefault();
            }
        }

        /// <summary>
        /// Update sync task result
        /// </summary>
        /// <param name="log"></param>
        public void UpdateLog(SyncTaskLog log)
        {
            lock (_syncTaskLogs)
            {
                if (!SyncTaskLogs.ContainsKey(log.ProcessType))
                {
                    SyncTaskLogs.Add(log.ProcessType, new List<SyncTaskLog>());
                }

                var existing = SyncTaskLogs[log.ProcessType].SingleOrDefault(x => x.Id == log.Id);
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
