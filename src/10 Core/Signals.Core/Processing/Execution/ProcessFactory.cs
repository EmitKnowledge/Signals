using Signals.Aspects.Bootstrap;
using Signals.Core.Business.Base;
using System;
using Signals.Core.Processing.Results;

namespace Signals.Core.Processing.Execution
{
    /// <summary>
    /// Process factory
    /// </summary>
    internal interface IProcessFactory
    {
        /// <summary>
        /// Create instance each time it is requested
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        IBaseProcess<TResponse> Create<TResponse>(Type type) where TResponse : VoidResult;
    }

    /// <summary>
    /// Process factory impl
    /// </summary>
    internal class ProcessFactory : IProcessFactory
    {
        /// <summary>
        /// Create instance each time it is requested
        /// </summary>
        /// <param name="processType"></param>
        /// <returns></returns>
        public IBaseProcess<TResponse> Create<TResponse>(Type processType) where TResponse : VoidResult
        {
            var instance = SystemBootstrapper.GetInstance(processType) as IBaseProcess<TResponse>;
            SystemBootstrapper.Bootstrap(instance);
            return instance;
        }
    }
}