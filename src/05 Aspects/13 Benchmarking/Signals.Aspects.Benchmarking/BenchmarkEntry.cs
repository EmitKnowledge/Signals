using System;
using System.Collections.Generic;
using System.Text;

namespace Signals.Aspects.Benchmarking
{
    /// <summary>
    /// Represents benchmarking entry
    /// </summary>
    public class BenchmarkEntry
    {
        /// <summary>
        /// Unique id of the entity
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Indicates when the records have been created in the system
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Correlation identifier for chaining
        /// </summary>
        public Guid CorrelationId { get; set; }

        /// <summary>
        /// Benchmarked process
        /// </summary>
        public string ProcessName { get; set; }

        /// <summary>
        /// Caller of benchmarked process
        /// </summary>
        public string CallerProcessName { get; set; }

        /// <summary>
        /// Key for benchmarking checkpoint
        /// </summary>
        public string Checkpoint { get; set; }

        /// <summary>
        /// Checkpoint description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Checkpoint payload
        /// </summary>
        public object Payload { get; set; }
    }
}
