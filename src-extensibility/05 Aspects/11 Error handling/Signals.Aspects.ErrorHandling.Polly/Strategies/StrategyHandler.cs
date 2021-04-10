using Polly;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Signals.Aspects.ErrorHandling.Polly.Strategies
{
    /// <summary>
    /// Polly error strategy handler
    /// </summary>
    public abstract class StrategyHandler : IStrategyHandler
    {
        /// <summary>
        /// Should automatically use defined strategy on processes
        /// </summary>
        public bool AutoHandleErrorProcesses { get; internal set; }
        
        /// <summary>
        /// Polly policy 
        /// </summary>
        internal Policy Policy { get; set; }

        /// <summary>
        /// Execute callback wrapped in an execution strategy
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        public abstract Task<TResult> Execute<TResult>(Func<TResult> action);
    }
}
