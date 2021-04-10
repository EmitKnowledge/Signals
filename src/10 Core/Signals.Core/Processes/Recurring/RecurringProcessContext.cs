using Signals.Aspects.DI;
using Signals.Aspects.DI.Attributes;
using Signals.Core.Processes.Base;
using Signals.Core.Common.Instance;
using System;
using System.Collections.Generic;
using Signals.Core.Processes.Recurring.Logging;
using Signals.Core.Processing.Results;

namespace Signals.Core.Processes.Recurring
{
    /// <summary>
    /// Recurring process context
    /// </summary>
    public interface IRecurringProcessContext : IBaseProcessContext
    {

        /// <summary>
        /// Recurring task log provider
        /// </summary>
        IRecurringTaskLogProvider RecurringTaskLogProvider { get; }

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

    /// <summary>
    /// Recurring process context
    /// </summary>
    [Export(typeof(IRecurringProcessContext))]
    public class RecurringProcessContext : BaseProcessContext, IRecurringProcessContext
    {
        /// <summary>
        /// Recurring task log provider
        /// </summary>
        [Import] public IRecurringTaskLogProvider RecurringTaskLogProvider { get; internal set; }

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