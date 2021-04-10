using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signals.Core.Web.Execution
{
    /// <summary>
    /// Represents execution state for middleware execution
    /// </summary>
    public enum MiddlewareResult
    {
        /// <summary>
        /// Result doesn't affect the middleware pipeline
        /// </summary>
        DoNothing,

        /// <summary>
        /// HttpRespose is already sent, don't execute other middlewares
        /// </summary>
        StopExecutionAndStopMiddlewarePipe,

        /// <summary>
        /// HttpRespose is not ser, execute other middlewares
        /// </summary>
        StopExecutionAndInvokeNextMiddleware
    }
}
