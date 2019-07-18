﻿using System;
using System.Collections.Generic;

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
        /// <param name="epicId"></param>
        /// <param name="description"></param>
        /// <param name="payload"></param>
        void Bench(string checkpointName, Guid epicId, string description = null, object payload = null);

        /// <summary>
        /// Persist epic data
        /// </summary>
        /// <param name="epicId"></param>
        void FlushEpic(Guid epicId);

        /// <summary>
        /// Get epic report data
        /// </summary>
        /// <param name="epicName"></param>
        Dictionary<Guid, List<BenchmarkEntry>> GetEpicReport(string epicName);

        /// <summary>
        /// Mark epic as started
        /// </summary>
        /// <param name="epicId"></param>
        /// <param name="epicName"></param>
        void StartEpic(Guid epicId, string epicName);
    }
}
