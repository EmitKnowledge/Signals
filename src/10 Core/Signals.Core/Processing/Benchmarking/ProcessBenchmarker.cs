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
            benchmarker.Bench(checkpointName, process.EpicId, process.Name, process.CallerProcessName, description, payload);
        }

        /// <summary>
        /// Persist epic data
        /// </summary>
        public void FlushEpic()
        {
            benchmarker.FlushEpic(process.EpicId);
        }

        /// <summary>
        /// Get epic report data
        /// </summary>
        /// <param name="epicName"></param>
        /// <param name="afterDate"></param>
        public EpicsReport GetEpicReport(string epicName, DateTime afterDate)
        {
            return benchmarker.GetEpicReport(epicName, afterDate);
        }

        /// <summary>
        /// Mark epic as started
        /// </summary>
        /// <param name="epicName"></param>
        public void StartEpic(string epicName)
        {
            benchmarker.StartEpic(process.EpicId, epicName);
        }
    }
}
