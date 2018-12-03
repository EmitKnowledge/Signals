using System;
using System.Collections.Generic;
using System.Text;

namespace Signals.Core.Processes.Recurring.Logging
{
    /// <summary>
    /// Synchronizaiton log provider
    /// </summary>
    public interface ISyncLogProvider
    {
        /// <summary>
        /// Insert sync task result
        /// </summary>
        /// <param name="log"></param>
        void CreateLog(SyncTaskLog log);

        /// <summary>
        /// Get current sync execution result if it is not finished
        /// </summary>
        /// <param name="processType"></param>
        /// <returns></returns>
        SyncTaskLog Current(Type processType);

        /// <summary>
        /// Get last sync execution results
        /// </summary>
        /// <param name="processType"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        List<SyncTaskLog> Last(Type processType, int take);

        /// <summary>
        /// Get last sync execution result
        /// </summary>
        /// <param name="processType"></param>
        /// <returns></returns>
        SyncTaskLog Last(Type processType);

        /// <summary>
        /// Update sync task result
        /// </summary>
        /// <param name="log"></param>
        void UpdateLog(SyncTaskLog log);
    }
}
