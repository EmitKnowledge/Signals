﻿using Signals.Core.Common.Instance;
using System.Collections.Generic;
using System.Linq;

namespace Signals.Core.Processes.Recurring.Logging
{
    public class SyncLogProvider : ISyncLogProvider
    {
        /// <summary>
        /// Logs repository
        /// </summary>
        private static List<SyncTaskLog> _syncTaskLogs;

        /// <summary>
        /// Logs repository provider
        /// </summary>
        private static List<SyncTaskLog> SyncTaskLogs
        {
            get
            {
                if (_syncTaskLogs.IsNull()) _syncTaskLogs = new List<SyncTaskLog>();
                return _syncTaskLogs;
            }
        }

        /// <summary>
        /// Insert sync task result
        /// </summary>
        /// <param name="log"></param>
        public void CreateLog(SyncTaskLog log)
        {
            SyncTaskLogs.Insert(0, log);
        }

        /// <summary>
        /// Get last sync execution results
        /// </summary>
        /// <param name="take"></param>
        /// <returns></returns>
        public List<SyncTaskLog> Last(int take)
        {
            return SyncTaskLogs.Take(take).ToList();
        }

        /// <summary>
        /// Get last sync execution result
        /// </summary>
        /// <returns></returns>
        public SyncTaskLog Last()
        {
            return SyncTaskLogs.FirstOrDefault();
        }
    }
}
