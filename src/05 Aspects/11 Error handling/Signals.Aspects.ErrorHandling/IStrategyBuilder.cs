using Signals.Aspects.ErrorHandling.Strategies;
using System;

namespace Signals.Aspects.ErrorHandling
{
    /// <summary>
    /// Execution strategy builder
    /// </summary>
    public interface IStrategyBuilder
    {
        /// <summary>
        /// Add strategies
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="strategy"></param>
        /// <returns></returns>
        IStrategyBuilder Add<TException>(Strategy strategy) where TException : Exception;

        /// <summary>
        /// Should automatically use defined strategy on processes
        /// </summary>
        /// <param name="shouldAutoHandle"></param>
        /// <returns></returns>
        IStrategyBuilder SetAutoHandling(bool shouldAutoHandle = true);

        /// <summary>
        /// Builds handler out of strategies
        /// </summary>
        /// <returns></returns>
        IStrategyHandler Build();


    }
}
