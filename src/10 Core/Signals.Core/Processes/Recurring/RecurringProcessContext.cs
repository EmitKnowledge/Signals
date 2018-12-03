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
        [Import] internal IRecurringTaskLogProvider RecurringTaskLogProvider { get; set; }

        /// <summary>
        /// Insert sync task result
        /// </summary>
        /// <param name="log"></param>
        public void CreateLog(RecurringTaskLog log)
        {
            RecurringTaskLogProvider?.CreateLog(log);
        }

        /// <summary>
        /// Get current sync execution result if it is not finished
        /// </summary>
        /// <param name="processType"></param>
        /// <returns></returns>
        public RecurringTaskLog Current(Type processType)
        {
            return RecurringTaskLogProvider?.Current(processType);
        }

        /// <summary>
        /// Get last sync execution results
        /// </summary>
        /// <param name="processType"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public List<RecurringTaskLog> Last(Type processType, int take)
        {
            return RecurringTaskLogProvider?.Last(processType, take);
        }

        /// <summary>
        /// Get last sync execution result
        /// </summary>
        /// <param name="processType"></param>
        /// <returns></returns>
        public RecurringTaskLog Last(Type processType)
        {
            return RecurringTaskLogProvider?.Last(processType);
        }

        /// <summary>
        /// Update sync task result
        /// </summary>
        /// <param name="log"></param>
        public void UpdateLog(RecurringTaskLog log)
        {
            RecurringTaskLogProvider?.UpdateLog(log);
        }
    }
}