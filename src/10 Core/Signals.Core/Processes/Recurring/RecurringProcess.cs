using Signals.Aspects.BackgroundProcessing.TaskConfiguration;
using Signals.Aspects.DI.Attributes;
using Signals.Core.Common.Exceptions;
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
        [Import]
        protected virtual IRecurringProcessContext Context
        {
            get => _context;
            set
            {
                (value as RecurringProcessContext)?.SetProcess(this); 
                _context = value;
            }
        }
        private IRecurringProcessContext _context;

        /// <summary>
        /// Base process context upcasted from Business process context
        /// </summary>
        internal override IBaseProcessContext BaseContext => Context;

        /// <summary>
        /// Recurring profile
        /// </summary>
        public abstract RecurrencePatternConfiguration Profile { get; }

        /// <summary>
        /// Checks if the recurring process should execute
        /// </summary>
        /// <returns></returns>
        public virtual bool ShouldExecute() => true;

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
            var log = new RecurringTaskLog
            {
                StartTime = DateTime.UtcNow,
                ProcessType = GetType()
            };

            Context.CreateLog(log);

            TResponse result = null;
            try
            {
                result = Sync();
                this.D("Executed -> Sync.");
                return result;
            }
            catch (Exception ex)
            {
				var dump = ExceptionsExtensions.Extract(
					ex,
					ExceptionsExtensions.ExceptionDetails.Type,
					ExceptionsExtensions.ExceptionDetails.Message,
					ExceptionsExtensions.ExceptionDetails.Stacktrace);
				this.D($"Exception has occurred while executing sync. Exception: {dump}.");
                result = VoidResult.Fail<TResponse>(ex);
                throw;
            }
            finally
            {
                log.EndTime = DateTime.UtcNow;
                log.Result = result;
                if (result != null)
                {
                    log.IsFaulted = result.IsFaulted || result.IsSystemFault;
                }
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
            if (!ShouldExecute())
            {
                return Ok();
            }
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
        [Import]
        protected virtual IRecurringProcessContext Context
        {
            get => _context;
            set
            {
                (value as RecurringProcessContext)?.SetProcess(this); 
                _context = value;
            }
        }

        private IRecurringProcessContext _context;

        /// <summary>
        /// Base process context upcasted from Business process context
        /// </summary>
        internal override IBaseProcessContext BaseContext => Context;

        /// <summary>
        /// Recurring profile
        /// </summary>
        public abstract RecurrencePatternConfiguration Profile { get; }

        /// <summary>
        /// Checks if the recurring process should execute
        /// </summary>
        /// <returns></returns>
        public virtual bool ShouldExecute() => true;

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
            var thisType = GetType();
            var currentExecuting = Context.Current(thisType);

            if (!currentExecuting.IsNull())
            {
                return Ok();
            }

            var log = new RecurringTaskLog
            {
                StartTime = DateTime.UtcNow,
                ProcessType = thisType
            };
            Context.CreateLog(log);

            TResponse result = null;
            try
            {
                result = Sync();
                this.D("Executed -> Sync.");
                return result;
            }
            catch (Exception ex)
            {
				var dump = ExceptionsExtensions.Extract(
					ex,
					ExceptionsExtensions.ExceptionDetails.Type,
					ExceptionsExtensions.ExceptionDetails.Message,
					ExceptionsExtensions.ExceptionDetails.Stacktrace);
				this.D($"Exception has occurred while executing sync. Exception: {dump}.");
	            result = VoidResult.Fail<TResponse>(ex);
                throw;
            }
            finally
            {
                log.EndTime = DateTime.UtcNow;
                log.Result = result;
                if (result != null)
                {
                    log.IsFaulted = result.IsFaulted || result.IsSystemFault;
                }
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
            if (!ShouldExecute())
            {
	            this.D("Process not allowed to be executed. Cancelling now.");
                return Ok();
            }
            return Execute();
        }
    }
}