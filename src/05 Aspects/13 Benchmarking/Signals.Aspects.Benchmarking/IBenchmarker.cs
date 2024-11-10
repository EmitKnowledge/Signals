using System;

namespace Signals.Aspects.Benchmarking
{
    /// <summary>
    /// Benchmarker
    /// </summary>
    public interface IBenchmarker
    {
		/// <summary>
		/// Hit benchmarking checkpoint
		/// </summary>
		/// <param name="checkpointName"></param>
		/// <param name="correlationId"></param>
		/// <param name="processName"></param>
		/// <param name="callerProcessName"></param>
		/// <param name="description"></param>
		/// <param name="payload"></param>
		void Bench(string checkpointName, Guid correlationId, string processName, string callerProcessName = null, string description = null, object payload = null);

		/// <summary>
		/// Persist epic data
		/// </summary>
		/// <param name="correlationId"></param>
		void Flush(Guid correlationId);

		/// <summary>
		/// Get correlation report data
		/// </summary>
		/// <param name="banchmarkName"></param>
		/// <param name="afterDate"></param>
		/// <returns></returns>
		BenchmarkReport GetReport(string banchmarkName, DateTime afterDate);

		/// <summary>
		/// Mark correlation as started
		/// </summary>
		/// <param name="correlationId"></param>
		/// <param name="banchmarkName"></param>
		void Start(Guid correlationId, string banchmarkName);
    }
}
