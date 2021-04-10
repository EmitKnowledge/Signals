using System;
using System.Collections.Generic;
using System.Text;

namespace Signals.Core.Processes.Recurring.Logging
{
    /// <summary>
    /// Synchronizaiton log provider
    /// </summary>
    public interface IRecurringTaskLogProvider
    {
        /// <summary>
        /// Insert sync task result
        /// </summary>
        /// <param name="log"></param>
        void CreateLog(RecurringTaskLog log);

        /// <summary>
        /// Get current sync execution result if it is not finished
        /// </summary>
        /// <param name="processType"></param>
        /// <returns></returns>
        RecurringTaskLog Current(Type processType);

        /// <summary>
        /// Get last sync execution results
        /// </summary>
        /// <param name="processType"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        List<RecurringTaskLog> Last(Type processType, int take);

        /// <summary>
        /// Get last sync execution result
        /// </summary>
        /// <param name="processType"></param>
        /// <returns></returns>
        RecurringTaskLog Last(Type processType);

        /// <summary>
        /// Update sync task result
        /// </summary>
        /// <param name="log"></param>
        void UpdateLog(RecurringTaskLog log);
    }
}
