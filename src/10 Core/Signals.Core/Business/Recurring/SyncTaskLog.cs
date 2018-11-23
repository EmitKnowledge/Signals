using Signals.Core.Processing.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Signals.Core.Business.Recurring
{
    /// <summary>
    /// Sync task execution result
    /// </summary>
    public class SyncTaskLog
    {
        /// <summary>
        /// Execution start time
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Execution end time
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Execution payload
        /// </summary>
        public VoidResult Result { get; set; }

        /// <summary>
        /// Has ended successfully
        /// </summary>
        public bool IsFaulted { get; set; }
    }
}
