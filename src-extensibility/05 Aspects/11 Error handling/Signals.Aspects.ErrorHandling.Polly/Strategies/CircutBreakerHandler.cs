using Polly;
using Polly.CircuitBreaker;
using Signals.Aspects.ErrorHandling.Strategies;
using System;
using System.Threading.Tasks;

namespace Signals.Aspects.ErrorHandling.Polly.Strategies
{
    /// <summary>
    /// Handler for circut strategy
    /// </summary>
    /// <typeparam name="TException"></typeparam>
    internal class CircutBreakerHandler<TException> : StrategyHandler where TException : Exception
    {
        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="strategy"></param>
        public CircutBreakerHandler(CircutBreakerStrategy strategy)
        {
            CircuitBreakerPolicy circutBreakerPolicy = Policy
                .Handle<TException>()
                .CircuitBreakerAsync(strategy.AllowedExceptionsCount, strategy.CooldownTimeout, (exception, count) => strategy.OnRetry?.Invoke(exception), strategy.OnReset);

            strategy.Reset = circutBreakerPolicy.Reset;
            strategy.Isolate = circutBreakerPolicy.Isolate;
            Policy = circutBreakerPolicy;
        }

        /// <summary>
        /// Execute callback wrapped in an execution strategy
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        public override async Task<TResult> Execute<TResult>(Func<TResult> action)
        {
            return await Policy.ExecuteAsync(() => Task.FromResult(action()));
        }
    }
}
