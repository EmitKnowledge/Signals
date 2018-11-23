using System;

namespace Signals.Aspects.ErrorHandling.Strategies
{
    /// <summary>
    /// Retry strategy
    /// </summary>
    public class RetryStrategy : Strategy
    {
        /// <summary>
        /// Strategy that will retry execution forever
        /// </summary>
        public static readonly RetryStrategy Forever = new RetryStrategy { RetryCount = -1 };

        /// <summary>
        /// CTOR
        /// </summary>
        public RetryStrategy()
        {
            RetryCooldown = TimeSpan.FromSeconds(0);
        }

        /// <summary>
        /// Number of times execution is retried
        /// </summary>
        public int RetryCount { get; set; }

        /// <summary>
        /// Period of time to wait between retries
        /// </summary>
        public TimeSpan RetryCooldown { get; set; }
    }
}