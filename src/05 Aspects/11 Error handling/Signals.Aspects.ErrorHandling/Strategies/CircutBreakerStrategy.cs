using System;

namespace Signals.Aspects.ErrorHandling.Strategies
{
    /// <summary>
    /// Circut breaker strategy
    /// </summary>
    public class CircutBreakerStrategy : Strategy
    {
        /// <summary>
        /// Number of exceptions that are allowed to happen before the strategy disallows to be executed again
        /// </summary>
        public int AllowedExceptionsCount { get; set; }

        /// <summary>
        /// Period of time for the circut to be broken after reaching maximum amount of exceptions
        /// </summary>
        public TimeSpan CooldownTimeout { get; set; }

        /// <summary>
        /// Callback for when the circut is reset
        /// </summary>
        public Action OnReset { get; set; }

        /// <summary>
        /// Handle for maually allowing execution
        /// </summary>
        public Action Reset { get; set; }

        /// <summary>
        /// Handle for maually disallowing execution
        /// </summary>
        public Action Isolate { get; set; }
    }
}
