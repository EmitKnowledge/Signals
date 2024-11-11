using Signals.Aspects.Benchmarking;
using Signals.Core.Processes.Base;
using Signals.Core.Processing.Results;
using System;

namespace Signals.Core.Processing.Benchmarking
{
    /// <summary>
    /// Process benchmarker
    /// </summary>
    public class ProcessBenchmarker
    {
        private readonly IBenchmarker benchmarker;
        private readonly IBaseProcess<VoidResult> process;

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="benchmarker"></param>
        /// <param name="process"></param>
        public ProcessBenchmarker(IBenchmarker benchmarker, IBaseProcess<VoidResult> process)
        {
            this.benchmarker = benchmarker;
            this.process = process;
        }

        /// <summary>
        /// Hit benchmarking checkpoint
        /// </summary>
        /// <param name="checkpointName"></param>
        /// <param name="description"></param>
        /// <param name="payload"></param>
        public void Bench(string checkpointName, string description = null, object payload = null)
        {
            benchmarker.Bench(checkpointName, process.CorrelationId, process.Name, process.CallerProcessName, description, payload);
            this.D($"Benchmarking for process type: {process?.GetType()?.FullName} -> Checkpoint: {checkpointName} -> CorrelationId : {process?.CorrelationId} -> Process name: {process?.Name} -> Process Caller Process Name: {process.CallerProcessName} -> Description: {description}.");
        }

		/// <summary>
		/// Persist correlation data
		/// </summary>
		public void FlushCorrelation()
        {
            benchmarker.Flush(process.CorrelationId);
            this.D($"Correlation with id: {process?.CorrelationId} for process type: {process?.GetType()?.FullName} flushed.");
        }

		/// <summary>
		/// Get correlation report data
		/// </summary>
		/// <param name="correlationName"></param>
		/// <param name="afterDate"></param>
		public BenchmarkReport GetCorrelationsReport(string correlationName, DateTime afterDate)
        {
            return benchmarker.GetReport(correlationName, afterDate);
        }

		/// <summary>
		/// Mark epic as started
		/// </summary>
		/// <param name="correlationName"></param>
		public void StartCorrelation(string correlationName)
        {
            benchmarker.Start(process.CorrelationId, correlationName);
            this.D($"Correlation: {correlationName} with id: {process?.CorrelationId} for process type: {process?.GetType()?.FullName} started.");
        }
    }
}
