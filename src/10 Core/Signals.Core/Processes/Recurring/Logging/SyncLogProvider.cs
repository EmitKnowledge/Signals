using Signals.Core.Common.Instance;
using System;
using System.Collections.Concurrent;
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
        private static ConcurrentDictionary<Type, ConcurrentBag<SyncTaskLog>> _syncTaskLogs;

        /// <summary>
        /// Logs repository provider
        /// </summary>
        private static ConcurrentDictionary<Type, ConcurrentBag<SyncTaskLog>> SyncTaskLogs
        {
            get
            {
                if (_syncTaskLogs.IsNull()) _syncTaskLogs = new ConcurrentDictionary<Type, ConcurrentBag<SyncTaskLog>>();
                return _syncTaskLogs;
            }
        }

        /// <summary>
        /// Insert sync task result
        /// </summary>
        /// <param name="log"></param>
        public void CreateLog(SyncTaskLog log)
        {
            var resultList = new ConcurrentBag<SyncTaskLog>();
            var hasValue = SyncTaskLogs.TryGetValue(log.ProcessType, out resultList);
            if (!hasValue || resultList.IsNull())
            {
                resultList = new ConcurrentBag<SyncTaskLog>();
                SyncTaskLogs.TryAdd(log.ProcessType, resultList);
            }

            lock (_syncTaskLogs)
            {
                log.Id = (resultList?.Count ?? 0) + 1;
            }

            resultList.Add(log);
        }

        /// <summary>
        /// Get current sync execution result if it is not finished
        /// </summary>
        /// <param name="processType"></param>
        /// <returns></returns>
        public SyncTaskLog Current(Type processType)
        {
            var hasValue = SyncTaskLogs.TryGetValue(processType, out ConcurrentBag<SyncTaskLog> list);

            if (!hasValue || list.IsNull())
            {
                return null;
            }

            return list.SingleOrDefault(x => !x.EndTime.HasValue);
        }

        /// <summary>
        /// Get last sync execution results
        /// </summary>
        /// <param name="processType"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public List<SyncTaskLog> Last(Type processType, int take)
        {
            var hasValue = SyncTaskLogs.TryGetValue(processType, out ConcurrentBag<SyncTaskLog> list);

            if (!hasValue || list.IsNull())
            {
                return null;
            }

            var last = list
                .OrderByDescending(x => x.EndTime)
                .Take(take)
                .ToList();

            return last;
        }

        /// <summary>
        /// Get last sync execution result
        /// </summary>
        /// <param name="processType"></param>
        /// <returns></returns>
        public SyncTaskLog Last(Type processType)
        {
            return Last(processType, 1).SingleOrDefault();
        }

        /// <summary>
        /// Update sync task result
        /// </summary>
        /// <param name="log"></param>
        public void UpdateLog(SyncTaskLog log)
        {
            var resultList = new ConcurrentBag<SyncTaskLog>();
            var hasValue = SyncTaskLogs.TryGetValue(log.ProcessType, out resultList);
            if (!hasValue || resultList.IsNull())
            {
                resultList = new ConcurrentBag<SyncTaskLog>();
                SyncTaskLogs.TryAdd(log.ProcessType, resultList);
            }

            var existing = resultList.SingleOrDefault(x => x.Id == log.Id);
            if (!existing.IsNull())
            {
                existing.Result = log.Result;
                existing.EndTime = log.EndTime;
                existing.IsFaulted = log.IsFaulted;
            }
        }
    }
}
