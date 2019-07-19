using Signals.Aspects.BackgroundProcessing.TaskConfiguration;
using Signals.Core.Common.Instance;
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
            Context = new RecurringProcessContext(this);
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
            RecurringTaskLog log = new RecurringTaskLog();
            log.StartTime = DateTime.UtcNow;
            log.ProcessType = this.GetType();
            Context.CreateLog(log);

            TResponse result = null;
            try
            {
                result = Sync();
                return result;
            }
            catch(Exception ex)
            {
                result = VoidResult.FaultedResult<TResponse>(ex);
                throw;
            }
            finally
            {
                log.EndTime = DateTime.UtcNow;
                log.Result = result;
                log.IsFaulted = result.IsFaulted || result.IsSystemFault;
                Context.UpdateLog(log);
            }
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

    /// <summary>
    /// Represents a process that should occur on a specified pattern
    /// Ex: Execute process every Monday at 20:00
    /// </summary>
    public abstract class NoOverlapRecurringProcess<TResponse> : BaseProcess<TResponse>, IRecurringProcess
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
        public NoOverlapRecurringProcess()
        {
            Context = new RecurringProcessContext(this);
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
            var thisType = this.GetType();
            var currentExecuting = Context.Current(thisType);

            if (!currentExecuting.IsNull()) return null;

            RecurringTaskLog log = new RecurringTaskLog();
            log.StartTime = DateTime.UtcNow;
            log.ProcessType = thisType;
            Context.CreateLog(log);

            TResponse result = null;
            try
            {
                result = Sync();
                return result;
            }
            catch (Exception ex)
            {
                result = VoidResult.FaultedResult<TResponse>(ex);
                throw;
            }
            finally
            {
                log.EndTime = DateTime.UtcNow;
                log.Result = result;
                log.IsFaulted = result.IsFaulted || result.IsSystemFault;
                Context.UpdateLog(log);
            }
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