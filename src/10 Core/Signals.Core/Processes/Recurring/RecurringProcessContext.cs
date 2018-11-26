using Signals.Aspects.Bootstrap;
using Signals.Aspects.DI.Attributes;
using Signals.Core.Processes.Base;
using Signals.Core.Common.Instance;
using System;
using System.Collections.Generic;

namespace Signals.Core.Processes.Recurring
{
    /// <summary>
    /// Recurring process context
    /// </summary>
    public class RecurringProcessContext : BaseProcessContext
    {
        [Import] internal ISyncLogProvider SyncLogProvider { get; set; }

        /// <summary>
        /// Insert sync task result in database
        /// </summary>
        /// <param name="log"></param>
        internal void CreateLog(SyncTaskLog log)
        {
            SyncLogProvider?.CreateLog(log);
        }

        /// <summary>
        /// Get last sync execution results
        /// </summary>
        /// <param name="take"></param>
        /// <returns></returns>
        public List<SyncTaskLog> Last(int take)
        {
            return SyncLogProvider?.Last(take);
        }

        /// <summary>
        /// Get last sync execution result
        /// </summary>
        /// <returns></returns>
        public SyncTaskLog Last()
        {
            return SyncLogProvider?.Last();
        }
    }
}