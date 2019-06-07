using Polly;
using Signals.Aspects.ErrorHandling.Strategies;
using System;
using System.Threading.Tasks;

namespace Signals.Aspects.ErrorHandling.Polly.Strategies
{
    /// <summary>
    /// Handler for retry strategy
    /// </summary>
    internal class EmptyHandler : StrategyHandler
    {
        /// <summary>
        /// CTOR
        /// </summary>
        public EmptyHandler()
        {
        }

        /// <summary>
        /// Execute callback wrapped in an execution strategy
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        public override async Task<TResult> Execute<TResult>(Func<TResult> action)
        {
            return await Task.FromResult(action());
        }
    }
}
