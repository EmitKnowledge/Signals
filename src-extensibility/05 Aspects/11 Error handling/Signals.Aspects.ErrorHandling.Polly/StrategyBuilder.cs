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
        /// 
        /// </summary>
        private bool ShouldAutoHandle { get; set; }

        /// <summary>
        /// CTOR
        /// </summary>
        public StrategyBuilder()
        {
            _strategyHandlers = new List<StrategyHandler>();
            ShouldAutoHandle = true;
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
            if (!_strategyHandlers.Any())
            {
                var _handler = new EmptyHandler();
                _handler.AutoHandleErrorProcesses = ShouldAutoHandle;
                return _handler;
            }
            else if (_strategyHandlers.Count > 1)
            {
                var _handler = new WrapHandler(_strategyHandlers.ToArray());
                _handler.AutoHandleErrorProcesses = ShouldAutoHandle;
                return _handler;
            }
            else if (_strategyHandlers.Count == 1)
            {
                var _handler = _strategyHandlers.First();
                _handler.AutoHandleErrorProcesses = ShouldAutoHandle;
                return _handler;
            }
            else
            {
                var _handler = new EmptyHandler();
                _handler.AutoHandleErrorProcesses = ShouldAutoHandle;
                return _handler;
            }
        }

        /// <summary>
        /// Should automatically use defined strategy on processes
        /// </summary>
        /// <param name="shouldAutoHandle"></param>
        /// <returns></returns>
        public IStrategyBuilder SetAutoHandling(bool shouldAutoHandle = true)
        {
            ShouldAutoHandle = shouldAutoHandle;
            return this;
        }
    }
}