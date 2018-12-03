using Signals.Core.Processing.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Signals.Core.Processes.Recurring
{
    /// <summary>
    /// Sync task execution result
    /// </summary>
    public class RecurringTaskLog
    {
        /// <summary>
        /// Entity id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Executing process type
        /// </summary>
        public Type ProcessType { get; set; }

        /// <summary>
        /// Execution start time
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Execution end time
        /// </summary>
        public DateTime? EndTime { get; set; }

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
