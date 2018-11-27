using NodaTime;
using NodaTime.Text;
using Signals.Aspects.BackgroundProcessing.TaskConfiguration;
using Signals.Core.Processes.Base;
using Signals.Core.Processing.Results;
using System;

namespace Signals.Core.Processes.Recurring
{
    /// <summary>
    /// Represents a recurring process
    /// </summary>
    internal interface IRecurringProcess
    {
        /// <summary>
        /// Recurring profile
        /// </summary>
        RecurrencePatternConfiguration Profile { get; }
    }

    /// <summary>
    /// Represents a process that should occur on a specified pattern
    /// Ex: Execute process every Monday at 20:00
    /// </summary>
    public abstract class RecurringProcess<TResponse> : BaseProcess<TResponse>, IRecurringProcess
        where TResponse : VoidResult, new()
    {
        /// <summary>
        /// Recurring process context
        /// </summary>
        protected virtual RecurringProcessContext Context { get; set; }

        /// <summary>
        /// Base process context upcasted from Business process context
        /// </summary>
        internal override BaseProcessContext BaseContext => Context;

        /// <summary>
        /// Recurring profile
        /// </summary>
        public abstract RecurrencePatternConfiguration Profile { get; }

        /// <summary>
        /// CTOR
        /// </summary>
        public RecurringProcess()
        {
            Context = new RecurringProcessContext();
        }

        /// <summary>
        /// Background execution layer
        /// </summary>
        /// <returns></returns>
        public abstract TResponse Sync();

        /// <summary>
        /// Execution using base strategy
        /// </summary>
        /// <returns></returns>
        internal TResponse Execute()
        {
            SyncTaskLog log = new SyncTaskLog();
            log.StartTime = DateTime.UtcNow;

            var result = Sync();

            log.EndTime = DateTime.UtcNow;
            log.Result = result;
            log.IsFaulted = result.IsFaulted || result.IsSystemFault;

            Context.CreateLog(log);

            return result;
        }

        /// <summary>
        /// Entry point executed by the factory
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        internal override TResponse ExecuteProcess(params object[] args)
        {
            return Execute();
        }
    }
}