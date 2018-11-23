using Signals.Aspects.ErrorHandling.Polly.Strategies;
using Signals.Aspects.ErrorHandling.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Signals.Aspects.ErrorHandling.Polly
{
    /// <summary>
    /// Execution strategy builder
    /// </summary>
    public class StrategyBuilder : IStrategyBuilder
    {
        /// <summary>
        /// Handlers
        /// </summary>
        private readonly List<StrategyHandler> _strategyHandlers;

        /// <summary>
        /// CTOR
        /// </summary>
        public StrategyBuilder()
        {
            _strategyHandlers = new List<StrategyHandler>();
        }

        /// <summary>
        /// Add strategies
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="strategy"></param>
        /// <returns></returns>
        public IStrategyBuilder Add<TException>(Strategy strategy) where TException : Exception
        {
            var handler = StrategyFactory.GetStrategyImplementation<TException>(strategy);
            _strategyHandlers.Add(handler);
            return this;
        }

        /// <summary>
        /// Builds handler out of strategies
        /// </summary>
        /// <returns></returns>
        public IStrategyHandler Build()
        {
            if (!_strategyHandlers.Any()) return null;

            if (_strategyHandlers.Count > 1)
            {
                return new WrapHandler(_strategyHandlers.ToArray());
            }

	        return _strategyHandlers.First();
        }
    }
}