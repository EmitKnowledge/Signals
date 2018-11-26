using Signals.Core.Processing.Execution;
using Signals.Core.Processing.Results;
using System;

namespace Signals.Core.Processes.Base
{
    /// <summary>
    /// Represents a base process
    /// </summary>
    public interface IBaseProcess<out TResponse>
        where TResponse : VoidResult
    {
	    /// <summary>
	    /// Process name
	    /// </summary>
	    string Name { get; set; }

	    /// <summary>
	    /// Process description
	    /// </summary>
	    string Description { get; set; }

	    /// <summary>
	    /// Base process context
	    /// </summary>
	    BaseProcessContext BaseContext { get; }

        /// <summary>
        /// Entry point executed by the factory
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        TResponse ExecuteProcess(params object[] args);
	}

    /// <summary>
    /// Represents a base process
    /// </summary>
    public abstract class BaseProcess<TResponse> : IBaseProcess<TResponse> 
	    where TResponse : VoidResult, new()
    {
        /// <summary>
        /// Process name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Process description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Base process context
        /// </summary>
        internal abstract BaseProcessContext BaseContext { get; }

	    /// <summary>
	    /// Base process context
	    /// </summary>
		BaseProcessContext IBaseProcess<TResponse>.BaseContext => BaseContext;

		/// <summary>
		/// CTOR
		/// </summary>
		protected BaseProcess()
        {
            Name = GetType().Name;
        }

		/// <summary>
		/// Entry point executed by the factory
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		internal abstract TResponse ExecuteProcess(params object[] args);

		/// <summary>
		/// Entry point executed by the factory
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		TResponse IBaseProcess<TResponse>.ExecuteProcess(params object[] args)
	    {
			return ExecuteProcess(args);
		}

        /// <summary>
        /// Execute chained business process
        /// </summary>
        /// <typeparam name="TProcess"></typeparam>
        /// <typeparam name="TInnerResponse"></typeparam>
        /// <returns></returns>
        protected TInnerResponse ContinueWith<TProcess, TInnerResponse>()
            where TProcess : BaseProcess<VoidResult>
            where TInnerResponse : VoidResult, new()
        {
            return Dispatch<TInnerResponse>(typeof(TProcess), null);
        }

        /// <summary>
        /// Execute chained business process
        /// </summary>
        /// <typeparam name="TProcess"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="TInnerResponse"></typeparam>
        /// <param name="obj1"></param>
        /// <returns></returns>
        protected TInnerResponse ContinueWith<TProcess, T1, TInnerResponse>(T1 obj1)
            where TProcess : BaseProcess<VoidResult>
            where TInnerResponse : VoidResult, new()
        {
            return Dispatch<TInnerResponse>(typeof(TProcess), obj1);
        }

        /// <summary>
        /// Execute chained business process
        /// </summary>
        /// <typeparam name="TProcess"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="TInnerResponse"></typeparam>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        /// <returns></returns>
        protected TInnerResponse ContinueWith<TProcess, T1, T2, TInnerResponse>(T1 obj1, T2 obj2)
            where TProcess : BaseProcess<VoidResult>
            where TInnerResponse : VoidResult, new()
        {
            return Dispatch<TInnerResponse>(typeof(TProcess), obj1, obj2);
        }

        /// <summary>
        /// Execute chained business process
        /// </summary>
        /// <typeparam name="TProcess"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="TInnerResponse"></typeparam>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        /// <param name="obj3"></param>
        /// <returns></returns>
        protected TInnerResponse ContinueWith<TProcess, T1, T2, T3, TInnerResponse>(T1 obj1, T2 obj2, T3 obj3)
            where TProcess : BaseProcess<VoidResult>
            where TInnerResponse : VoidResult, new()
        {
            return Dispatch<TInnerResponse>(typeof(TProcess), obj1, obj2, obj3);
        }

        /// <summary>
        /// Handler for chain-executing business processes
        /// </summary>
        /// <param name="type"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private TNewResponse Dispatch<TNewResponse>(Type type, params object[] args) where TNewResponse : VoidResult, new()
        {
            var process = BaseContext.ProcessFactory.Create<TNewResponse>(type);
            return BaseContext.ProcessExecutor.Execute((BaseProcess<TNewResponse>)process, args);
        }
    }
}