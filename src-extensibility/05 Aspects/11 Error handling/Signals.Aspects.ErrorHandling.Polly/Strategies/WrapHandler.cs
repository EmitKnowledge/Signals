using Polly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Signals.Aspects.ErrorHandling.Polly.Strategies
{
    /// <summary>
    /// Handler for wrap of strategies
    /// </summary>
    internal class WrapHandler : StrategyHandler
    {
        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="handlers"></param>
        public WrapHandler(params StrategyHandler[] handlers)
        {
            var policies = new List<Policy>();

            foreach (var handler in handlers)
            {
                policies.Add(handler.Policy);
            }

            Policy = Policy.WrapAsync(policies.ToArray());
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
