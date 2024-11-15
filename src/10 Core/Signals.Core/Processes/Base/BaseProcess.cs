﻿using Signals.Core.Processing.Exceptions;
using Signals.Core.Processing.Execution;
using Signals.Core.Processing.Results;
using Signals.Core.Processing.Specifications;
using System;
using System.Collections;
using System.Collections.Concurrent;

namespace Signals.Core.Processes.Base
{
	/// <summary>
	/// Represents a base process
	/// </summary>
	public interface IBaseProcess
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
		/// Processes correlation id
		/// </summary>
		Guid CorrelationId { get; set; }

        /// <summary>
        /// Caller process name
        /// </summary>
        string CallerProcessName { get; set; }

        /// <summary>
        /// Represents the stack of processes being excecuted
        /// </summary>
        Stack ExecutionStack { get; set; }

        /// <summary>
        /// Base process context
        /// </summary>
        IBaseProcessContext BaseContext { get; }
    }

    /// <summary>
    /// Represents a base process
    /// </summary>
    public interface IBaseProcess<out TResponse> : IBaseProcess
        where TResponse : VoidResult
    {
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
        /// Locks container
        /// </summary>
        private static readonly ConcurrentDictionary<string, object> _locksDictionary = new ConcurrentDictionary<string, object>();

        /// <summary>
        /// Statically locks all instances of current process
        /// </summary>
        public object SpinLock => _locksDictionary.GetOrAdd(Name, _ => new object());

        /// <summary>
        /// Process name
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Process description
        /// </summary>
        public virtual string Description { get; set; }

		/// <summary>
		/// Processes correlation id
		/// </summary>
		public virtual Guid CorrelationId { get; set; }

        /// <summary>
        /// Caller process name
        /// </summary>
        public string CallerProcessName { get; set; }

        /// <summary>
        /// Represents the stack of processes being excecuted
        /// </summary>
        public Stack ExecutionStack { get; set; }

        /// <summary>
        /// Base process context
        /// </summary>
        internal abstract IBaseProcessContext BaseContext { get; }

        /// <summary>
        /// Base process context
        /// </summary>
        IBaseProcessContext IBaseProcess.BaseContext => BaseContext;

        /// <summary>
        /// CTOR
        /// </summary>
        protected BaseProcess()
        {
            Name = GetType().Name;
			CorrelationId = Guid.NewGuid();
            ExecutionStack = new Stack();
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
            var stackEntry = new ProcessExecutionStackEntry
            {
                Process = this,
                Payload = args
            };
            ExecutionStack.Push(stackEntry);
            return ExecuteProcess(args);
        }

        /// <summary>
        /// Return rule engine for validation
        /// </summary>
        /// <returns></returns>
        protected RuleEngine<TResponse> BeginValidation()
        {
            return new RuleEngine<TResponse>();
        }

        /// <summary>
        /// Return empty result
        /// </summary>
        /// <returns></returns>
        protected TResponse Ok()
        {
            return new TResponse();
        }

        /// <summary>
        /// Return faulted result
        /// </summary>
        /// <param name="failCause"></param>
        /// <returns></returns>
        protected TResponse Fail(Exception failCause)
        {
            return VoidResult.FaultedResult<TResponse>(failCause);
        }

        /// <summary>
        /// Return faulted result
        /// </summary>
        /// <param name="failCauses"></param>
        /// <returns></returns>
        protected TResponse Fail(params IErrorInfo[] failCauses)
        {
            return VoidResult.FaultedResult<TResponse>(failCauses);
        }

        /// <summary>
        /// Return faulted result
        /// </summary>
        /// <returns></returns>
        protected TResponse Fail()
        {
            return VoidResult.FaultedResult<TResponse>();
        }

        /// <summary>
        /// Return faulted result
        /// </summary>
        /// <param name="voidResult"></param>
        /// <returns></returns>
        protected TResponse Fail(VoidResult voidResult)
        {
            return VoidResult.FaultedResult<TResponse>(voidResult?.ErrorMessages?.ToArray());
        }

        /// <summary>
        /// Execute chained business process
        /// </summary>
        /// <typeparam name="TProcess"></typeparam>
        /// <typeparam name="TInnerResponse"></typeparam>
        /// <returns></returns>
        protected TInnerResponse ContinueWith<TProcess, TInnerResponse>()
            where TProcess : IBaseProcess<TInnerResponse>
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
            where TProcess : IBaseProcess<TInnerResponse>
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
            where TProcess : IBaseProcess<TInnerResponse>
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
            where TProcess : IBaseProcess<TInnerResponse>
            where TInnerResponse : VoidResult, new()
        {
            return Dispatch<TInnerResponse>(typeof(TProcess), obj1, obj2, obj3);
        }

        /// <summary>
        /// Execute chained business process
        /// </summary>
        /// <typeparam name="TProcess"></typeparam>
        /// <returns></returns>
        protected TProcess Continue<TProcess>()
            where TProcess : class, IBaseProcess<VoidResult>, new()
        {
            var processType = typeof(TProcess);
            var process = BaseContext.Mediator.ProcessFactory.Create<VoidResult>(processType);

            process.CorrelationId = CorrelationId;
            process.CallerProcessName = Name;
            process.ExecutionStack = ExecutionStack;

            return process as TProcess;
        }

        /// <summary>
        /// Handler for chain-executing business processes
        /// </summary>
        /// <param name="type"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private TNewResponse Dispatch<TNewResponse>(Type type, params object[] args) where TNewResponse : VoidResult, new()
        {
            var process = BaseContext.Mediator.ProcessFactory.Create<TNewResponse>(type);

            process.CorrelationId = CorrelationId;
            process.CallerProcessName = Name;
            process.ExecutionStack = ExecutionStack;

            return BaseContext.Mediator.ProcessExecutor.Execute((BaseProcess<TNewResponse>)process, args);
        }
    }
}