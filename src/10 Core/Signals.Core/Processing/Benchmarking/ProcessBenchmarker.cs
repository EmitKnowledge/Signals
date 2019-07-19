using Signals.Aspects.Benchmarking;
using Signals.Core.Processes.Base;
using Signals.Core.Processing.Results;
using System;
using System.Collections.Generic;

namespace Signals.Core.Processing.Benchmarking
{
    /// <summary>
    /// Process benchmarker
    /// </summary>
    public class ProcessBenchmarker : IBenchmarker
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
            benchmarker.Bench(checkpointName, process.EpicId, process.Name, process.CallerProcessName, description, payload);
        }

        /// <summary>
        /// Hit benchmarking checkpoint
        /// </summary>
        /// <param name="checkpointName"></param>
        /// <param name="epicId"></param>
        /// <param name="processName"></param>
        /// <param name="callerProcessName"></param>
        /// <param name="description"></param>
        /// <param name="payload"></param>
        public void Bench(string checkpointName, Guid epicId, string processName, string callerProcessName = null, string description = null, object payload = null)
        {
            benchmarker.Bench(checkpointName, epicId, processName, callerProcessName, description, payload);
        }

        /// <summary>
        /// Persist epic data
        /// </summary>
        /// <param name="epicId"></param>
        public void FlushEpic(Guid epicId)
        {
            benchmarker.FlushEpic(epicId);
        }

        /// <summary>
        /// Get epic report data
        /// </summary>
        /// <param name="epicName"></param>
        /// <param name="afterDate"></param>
        public Dictionary<Guid, List<BenchmarkEntry>> GetEpicReport(string epicName, DateTime afterDate)
        {
            return benchmarker.GetEpicReport(epicName, afterDate);
        }

        /// <summary>
        /// Mark epic as started
        /// </summary>
        /// <param name="epicId"></param>
        /// <param name="epicName"></param>
        public void StartEpic(Guid epicId, string epicName)
        {
            benchmarker.StartEpic(epicId, epicName);
        }
    }
}
