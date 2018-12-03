using Signals.Aspects.DI;
using Signals.Aspects.DI.Attributes;
using Signals.Core.Processes.Base;
using Signals.Core.Common.Instance;
using System;
using System.Collections.Generic;
using Signals.Core.Processes.Recurring.Logging;

namespace Signals.Core.Processes.Recurring
{
    /// <summary>
    /// Recurring process context
    /// </summary>
    public class RecurringProcessContext : BaseProcessContext
    {
        [Import] internal ISyncLogProvider SyncLogProvider { get; set; }

        /// <summary>
        /// Insert sync task result
        /// </summary>
        /// <param name="log"></param>
        public void CreateLog(SyncTaskLog log)
        {
            SyncLogProvider?.CreateLog(log);
        }

        /// <summary>
        /// Get current sync execution result if it is not finished
        /// </summary>
        /// <param name="processType"></param>
        /// <returns></returns>
        public SyncTaskLog Current(Type processType)
        {
            return SyncLogProvider?.Current(processType);
        }

        /// <summary>
        /// Get last sync execution results
        /// </summary>
        /// <param name="processType"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public List<SyncTaskLog> Last(Type processType, int take)
        {
            return SyncLogProvider?.Last(processType, take);
        }

        /// <summary>
        /// Get last sync execution result
        /// </summary>
        /// <param name="processType"></param>
        /// <returns></returns>
        public SyncTaskLog Last(Type processType)
        {
            return SyncLogProvider?.Last(processType);
        }

        /// <summary>
        /// Update sync task result
        /// </summary>
        /// <param name="log"></param>
        public void UpdateLog(SyncTaskLog log)
        {
            SyncLogProvider?.UpdateLog(log);
        }
    }
}