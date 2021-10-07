using Signals.Aspects.BackgroundProcessing;
using Signals.Aspects.DI;
using Signals.Core.Processes.Base;
using Signals.Core.Processing.Execution;
using Signals.Core.Processing.Results;
using System;

namespace Signals.Core.Processes.Recurring
{
    /// <summary>
    /// Wrapper for recurring processes to adapt to <see cref="ISyncTask"/>
    /// </summary>
    public class RecurringTaskWrapper : ISyncTask
    {
        /// <summary>
        /// Wrapper process
        /// </summary>
        internal readonly Type InnerProcessType;

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="processType"></param>
        public RecurringTaskWrapper(Type processType)
        {
            InnerProcessType = processType;
            this.D($"Initializing RecurringTaskWrapper for {processType?.FullName}.");
        }

        /// <summary>
        /// <see cref="ISyncTask"/> execution handle
        /// </summary>
        public void Execute()
        {
            var factor = SystemBootstrapper.GetInstance<IProcessFactory>();
            var executor = SystemBootstrapper.GetInstance<IProcessExecutor>();

            var instance = factor.Create<VoidResult>(InnerProcessType);

            var executionResult = executor.Execute(instance);
            var message = $"Executed recurring task of type {InnerProcessType?.FullName}.";
            this.D(executionResult.IsFaulted
	            ? $"{message} Fault: {executionResult?.GetFaultMessage()}."
	            : message);
        }
    }
}