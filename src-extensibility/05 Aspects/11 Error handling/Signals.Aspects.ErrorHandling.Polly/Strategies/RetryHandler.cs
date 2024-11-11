using Polly;
using Signals.Aspects.ErrorHandling.Strategies;
using System;
using System.Threading.Tasks;

namespace Signals.Aspects.ErrorHandling.Polly.Strategies
{
    /// <summary>
    /// Handler for retry strategy
    /// </summary>
    /// <typeparam name="TException"></typeparam>
    internal class RetryHandler<TException> : StrategyHandler where TException : Exception
    {
        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="strategy"></param>
        public RetryHandler(RetryStrategy strategy)
        {
            if (strategy.RetryCount >= 0)
            {
                Policy = Policy
                    .Handle<TException>()
                    .WaitAndRetry(strategy.RetryCount, count => strategy.RetryCooldown, (exception, count) => strategy.OnRetry?.Invoke(exception));
            }
            else
            {
                Policy = Policy
                    .Handle<TException>()
                    .WaitAndRetryForever(count => strategy.RetryCooldown, (exception, time) => strategy.OnRetry?.Invoke(exception));
            }
        }

        /// <summary>
        /// Execute callback wrapped in an execution strategy
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        public override async Task<TResult> Execute<TResult>(Func<TResult> action)
        {
            return await Policy.Execute(() => Task.FromResult(action()));
        }
    }
}
