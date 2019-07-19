using Signals.Core.Processing.Input;
using System;

namespace Signals.Core.Processes.Distributed
{
    /// <summary>
    /// Metadata for distributed process
    /// </summary>
    public class DistributedProcessMetadata
    {
        /// <summary>
        /// Id of epic execution chain
        /// </summary>
        public Guid EpicId { get; set; }

        /// <summary>
        /// Commiunication payload
        /// </summary>
        public string Payload { get; set; }
    }
}
