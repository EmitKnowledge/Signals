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
    /// <summary>
    /// Execution extensions for mediator
    /// </summary>
    public static class MediatorExtensions
    {
        /// <summary>
        /// Execute process with input
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="process"></param>
        /// <returns></returns>
        public static TResponse With<TResponse>(this IBaseProcess<TResponse> process)
            where TResponse : VoidResult, new()
        {
            return process.BaseContext.Mediator.Dispatch(process);
        }

        /// <summary>
        /// Execute process with input
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="process"></param>
        /// <param name="input1"></param>
        /// <returns></returns>
        public static TResponse With<T1, TResponse>(this IBaseProcess<TResponse> process, T1 input1)
            where TResponse : VoidResult, new()
        {
            return process.BaseContext.Mediator.Dispatch(process, input1);
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
        public static TResponse With<T1, T2, TResponse>(this IBaseProcess<TResponse> process, T1 input1, T2 input2)
            where TResponse : VoidResult, new()
        {
            return process.BaseContext.Mediator.Dispatch(process, input1, input2);
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
        public static TResponse With<T1, T2, T3, TResponse>(this IBaseProcess<TResponse> process, T1 input1, T2 input2, T3 input3)
            where TResponse : VoidResult, new()
        {
            return process.BaseContext.Mediator.Dispatch(process, input1, input2, input3);
        }
    }
}
