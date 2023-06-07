using System;
using Signals.Core.Processes.Base;

namespace Signals.Core.Processing.Execution
{
    /// <summary>
    /// Represents the process execution stack necessary data
    /// </summary>
    internal class ProcessExecutionStackEntry
    {
        /// <summary>
        /// Represents the process in the execution stack
        /// </summary>
        internal IBaseProcess Process { get; set; }

        /// <summary>
        /// Represents the serialized process input payload
        /// </summary>
        internal object[] Payload { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        public ProcessExecutionStackEntry()
        {
            Payload = Array.Empty<object>();
        }
    }
}
