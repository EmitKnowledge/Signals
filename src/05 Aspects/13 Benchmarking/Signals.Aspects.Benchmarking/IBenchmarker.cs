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
        /// <param name="epic"></param>
        /// <param name="description"></param>
        void Bench(string checkpointName, string epic, string description);

        /// <summary>
        /// Hit benchmarking checkpoint
        /// </summary>
        /// <param name="checkpointName"></param>
        /// <param name="epic"></param>
        /// <param name="description"></param>
        void Bench(Enum checkpointName, Enum epic, string description);

        /// <summary>
        /// Mark epic as started
        /// </summary>
        /// <param name="epicName"></param>
        void StartEpic(string epicName);

        /// <summary>
        /// Mark epic as ended
        /// </summary>
        /// <param name="epicName"></param>
        void EndEpic(string epicName);

        /// <summary>
        /// Persist epic data
        /// </summary>
        /// <param name="epicName"></param>
        void FlushEpic(string epicName);
    }
}
