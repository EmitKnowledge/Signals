using System;
using System.Collections.Generic;
using System.Text;
using Signals.Aspects.Auditing;
using Signals.Aspects.DI;
using Signals.Core.Processes.Base;
using Signals.Core.Common.Instance;
using Signals.Core.Configuration;
using Signals.Core.Processing.Results;
using Signals.Core.Common.Serialization;

namespace Signals.Core.Processing.Execution.ExecutionHandlers
{
    /// <summary>
    /// Process execution handler
    /// </summary>
    internal class AuditingHandler : IExecutionHandler
    {
        /// <summary>
        /// Next handler in execution pipe
        /// </summary>
        public IExecutionHandler Next { get; set; }

        /// <summary>
        /// Execute handler
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="process"></param>
        /// <param name="processType"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public TResult Execute<TResult>(IBaseProcess<TResult> process, Type processType, params object[] args) where TResult : VoidResult, new()
        {
            var auditProvider = SystemBootstrapper.GetInstance<IAuditProvider>();
            if (auditProvider.IsNull()) return Next.Execute(process, processType, args);

            var entry = auditProvider.Entry();
            entry.Originator = ApplicationConfiguration.Instance?.ApplicationName ?? Environment.MachineName;
            entry.Process = process.Name;
            entry.EventType = process.Description;
            entry.EpicId = process.EpicId;
            entry.Payload = args?.SerializeJson();

            return auditProvider.Audit(entry, () => Next.Execute(process, processType, args));
        }
    }
}
