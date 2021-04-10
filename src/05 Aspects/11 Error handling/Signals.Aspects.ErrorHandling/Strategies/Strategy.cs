using System;

namespace Signals.Aspects.ErrorHandling.Strategies
{
    /// <summary>
    /// Represents execution strategy
    /// </summary>
    public abstract class Strategy
    {

        /// <summary>
        /// Callback handler for when an exception occures
        /// </summary>
        public Action<Exception> OnRetry { get; set; }
    }
}
