﻿using Newtonsoft.Json;
using Signals.Core.Processes.Base;
using Signals.Core.Processes.Business;
using Signals.Core.Common.Serialization;
using Signals.Core.Processing.Input;
using Signals.Core.Processing.Results;
using System;
using System.Threading.Tasks;

namespace Signals.Core.Processes.Distributed
{
    /// <summary>
    /// Represents a distributed process
    /// </summary>
    internal interface IDistributedProcess { VoidResult ExecuteBackgroundProcess(params object[] args); }

    /// <summary>
    /// Represents a usecase
    /// </summary>
    public abstract class DistributedProcess<TTransientData, TResponse> : BusinessProcess<TResponse>,
        IDistributedProcess
        where TResponse : VoidResult, new()
        where TTransientData : ITransientData
    {
        /// <summary>
        /// Business process context
        /// </summary>
        protected new virtual DistributedProcessContext Context { get; set; }

        /// <summary>
        /// Base process context upcasted from Api process context
        /// </summary>
        internal override BaseProcessContext BaseContext => Context;

        /// <summary>
        /// CTOR
        /// </summary>
        public DistributedProcess()
        {
            Context = new DistributedProcessContext();
        }

        /// <summary>
        /// Execution layer
        /// </summary>
        /// <returns></returns>
        public abstract VoidResult Work(TTransientData request);

        /// <summary>
        /// Execution layer
        /// </summary>
        /// <returns></returns>
        public abstract TTransientData Map(TResponse response);

        /// <summary>
        /// Execution using base strategy
        /// </summary>
        /// <returns></returns>
        internal override TResponse Execute()
        {
            var result = base.Execute();
            if (result.IsFaulted) return result;

            PublishNotificaiton(Name, Map(result));

            return result;
        }

        /// <summary>
        /// Entry point executed by the factory
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        internal override TResponse ExecuteProcess(params object[] args)
        {
            return Execute();
        }

        /// <summary>
        /// Entry point executed by the factory
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        internal VoidResult ExecuteBackgroundProcess(params object[] args)
        {
            var transientData = (args[0] as string).Deserialize<TTransientData>(SerializationFormat.Json);
            return Work(transientData);
        }

        VoidResult IDistributedProcess.ExecuteBackgroundProcess(params object[] args)
        {
            return ExecuteBackgroundProcess(args);
        }

        /// <summary>
        /// Handler for publishing events to communicaiton channel
        /// </summary>
        /// <param name="name"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal Task PublishNotificaiton(string name, object obj)
        {
            return Context.Channel?.Publish(name, obj);
        }
    }

    /// <summary>
    /// Represents a usecase
    /// </summary>
    public abstract class DistributedProcess<TTransientData, TRequest, TResponse> : BusinessProcess<TRequest, TResponse>,
        IDistributedProcess
        where TRequest : IDtoData
        where TResponse : VoidResult, new()
        where TTransientData : ITransientData
    {
        /// <summary>
        /// Business process context
        /// </summary>
        protected new virtual DistributedProcessContext Context { get; set; }

        /// <summary>
        /// Base process context upcasted from Api process context
        /// </summary>
        internal override BaseProcessContext BaseContext => Context;

        /// <summary>
        /// CTOR
        /// </summary>
        public DistributedProcess()
        {
            Context = new DistributedProcessContext();
        }

        /// <summary>
        /// Execution layer
        /// </summary>
        /// <returns></returns>
        public abstract VoidResult Work(TTransientData request);

        /// <summary>
        /// Execution layer
        /// </summary>
        /// <returns></returns>
        public abstract TTransientData Map(TRequest request, TResponse response);

        /// <summary>
        /// Execution using base strategy
        /// </summary>
        /// <returns></returns>
        internal override TResponse Execute(TRequest request)
        {
            var result = base.Execute(request);
            if (result.IsFaulted) return result;

            PublishNotificaiton(Name, Map(request, result));
            return result;
        }

        /// <summary>
        /// Entry point executed by the factory
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        internal override TResponse ExecuteProcess(params object[] args)
        {
            if (args.Length > 1 && args[0] is string inputStr)
            {
                if (inputStr == "BG")
                {
                    var input = JsonConvert.DeserializeObject<TTransientData>(args[1] as string);
                    Work(input);

                    return new TResponse();
                }
            }

            if (args[0] is TRequest obj)
                return Execute(obj);

            var fallbackInput = JsonConvert.DeserializeObject<TRequest>(args[0] as string);
            return Execute(fallbackInput);
        }

        /// <summary>
        /// Entry point executed by the factory
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        internal VoidResult ExecuteBackgroundProcess(params object[] args)
        {
            var transientData = (args[0] as string).Deserialize<TTransientData>(SerializationFormat.Json);
            return Work(transientData);
        }

        VoidResult IDistributedProcess.ExecuteBackgroundProcess(params object[] args)
        {
            return ExecuteBackgroundProcess(args);
        }

        /// <summary>
        /// Handler for publishing events to communicaiton channel
        /// </summary>
        /// <param name="name"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal Task PublishNotificaiton(string name, object obj)
        {
            return Context.Channel?.Publish(name, obj);
        }
    }
}
