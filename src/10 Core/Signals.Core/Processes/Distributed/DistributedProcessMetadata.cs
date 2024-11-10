using System;

namespace Signals.Core.Processes.Distributed
{
    /// <summary>
    /// Metadata for distributed process
    /// </summary>
    public class DistributedProcessMetadata
    {
		/// <summary>
		/// Id of correlation execution chain
		/// </summary>
		public Guid CorrelationId { get; set; }

        /// <summary>
        /// Commiunication payload
        /// </summary>
        public string Payload { get; set; }

        /// <summary>
        /// Thread culture name
        /// </summary>
        public string CultureName { get; set; }

        /// <summary>
        /// Caller process name
        /// </summary>
        public string CallerProcessName { get; set; }
    }
}
