using Signals.Aspects.ErrorHandling.Strategies;
using Signals.Aspects.ErrorHandling.Polly.Strategies;
using System;

namespace Signals.Aspects.ErrorHandling.Polly
{
    /// <summary>
    /// Execution strategy factory
    /// </summary>
    internal static class StrategyFactory
    {
        /// <summary>
        /// Resolves handlers from strategies
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="strategy"></param>
        /// <returns></returns>
        public static StrategyHandler GetStrategyImplementation<TException>(Strategy strategy) where TException : Exception
        {
            if (strategy is RetryStrategy retryStrategy) return new RetryHandler<TException>(retryStrategy);
            if (strategy is CircutBreakerStrategy circutBreakerStrategy) return new CircutBreakerHandler<TException>(circutBreakerStrategy);
            
            throw new Exception("Strategy not supported");
        }
    }
}
