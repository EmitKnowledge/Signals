using System;
using System.Collections.Generic;
using System.Text;

namespace Signals.Aspects.Logging.Enums
{
    /// <summary>
    /// Logging level
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Finer-grained informational events than the DEBUG
        /// </summary>
        Trace = 1,

        /// <summary>
        /// Fine-grained informational events that are most useful to debug an application
        /// </summary>
        Debug = 2,

        /// <summary>
        /// Informational messages that highlight the progress of the application at coarse-grained level
        /// </summary>
        Info = 3,

        /// <summary>
        /// Potentially harmful situations
        /// </summary>
        Warn = 4,

        /// <summary>
        /// Error events that might still allow the application to continue running
        /// </summary>
        Error = 5,

        /// <summary>
        /// Very severe error events that will presumably lead the application to abort
        /// </summary>
        Fatal = 6
    }
}
