using Signals.Aspects.DI;
using Signals.Core.Processes.Base;
using Signals.Core.Processes.Business;
using Signals.Core.Processing.Execution;
using Signals.Core.Processing.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Signals.Core.Processes
{
    public static class MediatorExtensions
    {
        /// <summary>
        /// Execute process with input
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="process"></param>
        /// <returns></returns>
        public static TResponse With<TResponse>(this BusinessProcess<TResponse> process)
            where TResponse : VoidResult, new()
        {
            var mediator = SystemBootstrapper.GetInstance<Mediator>();
            return mediator.Dispatch<TResponse>(process.GetType());
        }

        /// <summary>
        /// Execute process with input
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="process"></param>
        /// <param name="input1"></param>
        /// <returns></returns>
        public static TResponse With<T1, TResponse>(this BusinessProcess<T1, TResponse> process, T1 input1)
            where TResponse : VoidResult, new()
        {
            var mediator = SystemBootstrapper.GetInstance<Mediator>();
            return mediator.Dispatch<T1, TResponse>(process.GetType(), input1);
        }

        /// <summary>
        /// Execute process with input
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="process"></param>
        /// <param name="input1"></param>
        /// <param name="input2"></param>
        /// <returns></returns>
        public static TResponse With<T1, T2, TResponse>(this BusinessProcess<T1, T2, TResponse> process, T1 input1, T2 input2)
            where TResponse : VoidResult, new()
        {
            var mediator = SystemBootstrapper.GetInstance<Mediator>();
            return mediator.Dispatch<T1, T2, TResponse>(process.GetType(), input1, input2);
        }

        /// <summary>
        /// Execute process with input
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="process"></param>
        /// <param name="input1"></param>
        /// <param name="input2"></param>
        /// <param name="input3"></param>
        /// <returns></returns>
        public static TResponse With<T1, T2, T3, TResponse>(this BusinessProcess<T1, T2, T3, TResponse> process, T1 input1, T2 input2, T3 input3)
            where TResponse : VoidResult, new()
        {
            var mediator = SystemBootstrapper.GetInstance<Mediator>();
            return mediator.Dispatch<T1, T2, T3, TResponse>(process.GetType(), input1, input2, input3);
        }

        /// <summary>
        /// Specify process to invoke
        /// </summary>
        /// <typeparam name="TProcess"></typeparam>
        /// <param name="mediator"></param>
        /// <returns></returns>
        public static TProcess For<TProcess>(this Mediator mediator)
            where TProcess : class, IBaseProcess<VoidResult>, new()
        {
            var factory = SystemBootstrapper.GetInstance<IProcessFactory>();
            var processType = typeof(TProcess);
            var instance = factory.Create<VoidResult>(processType) as TProcess;
            return instance;
        }
    }
}
