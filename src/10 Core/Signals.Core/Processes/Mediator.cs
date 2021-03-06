﻿using Signals.Aspects.DI.Attributes;
using Signals.Core.Common.Instance;
using Signals.Core.Processes.Base;
using Signals.Core.Processes.Business;
using Signals.Core.Processing.Execution;
using Signals.Core.Processing.Results;
using System;

namespace Signals.Core.Processes
{
    /// <summary>
    /// Manual request mediator
    /// </summary>
    public class Mediator
    {
        /// <summary>
        /// Process factory
        /// </summary>
        [Import] internal IProcessFactory ProcessFactory { get; set; }

        /// <summary>
        /// Process executor
        /// </summary>
        [Import] internal IProcessExecutor ProcessExecutor { get; set; }

        /// <summary>
        /// Handle the request
        /// </summary>
        /// <returns></returns>
        public TResponse Dispatch<TResponse>(Type processType)
            where TResponse : VoidResult, new()
        {
            // create instance
            var process = ProcessFactory.Create<TResponse>(processType);
            if (process.IsNull()) return VoidResult.FaultedResult<TResponse>();

            // execute process
            var response = ProcessExecutor.Execute(process);

            return response;
        }

        /// <summary>
        /// Handle the request
        /// </summary>
        /// <returns></returns>
        public TResponse Dispatch<T1, TResponse>(Type processType, T1 obj1)
            where TResponse : VoidResult, new()
        {
            // create instance
            var process = ProcessFactory.Create<TResponse>(processType);
            if (process.IsNull()) return VoidResult.FaultedResult<TResponse>();

            // execute process
            var response = ProcessExecutor.Execute(process, obj1);

            return response;
        }

        /// <summary>
        /// Handle the request
        /// </summary>
        /// <returns></returns>
        public TResponse Dispatch<T1, T2, TResponse>(Type processType, T1 obj1, T2 obj2)
            where TResponse : VoidResult, new()
        {
            // create instance
            var process = ProcessFactory.Create<TResponse>(processType);
            if (process.IsNull()) return VoidResult.FaultedResult<TResponse>();

            // execute process
            var response = ProcessExecutor.Execute(process, obj1, obj2);

            return response;
        }

        /// <summary>
        /// Handle the request
        /// </summary>
        /// <returns></returns>
        public TResponse Dispatch<T1, T2, T3, TResponse>(Type processType, T1 obj1, T2 obj2, T3 obj3)
            where TResponse : VoidResult, new()
        {
            // create instance
            var process = ProcessFactory.Create<TResponse>(processType);
            if (process.IsNull()) return VoidResult.FaultedResult<TResponse>();

            // execute process
            var response = ProcessExecutor.Execute(process, obj1, obj2, obj3);

            return response;
        }

        /// <summary>
        /// Handle the request
        /// </summary>
        /// <returns></returns>
        public TResponse Dispatch<TProcess, TResponse>()
            where TProcess : IBaseProcess<TResponse>, new()
            where TResponse : VoidResult, new()
        {
            // create instance
            var process = ProcessFactory.Create<TResponse>(typeof(TProcess));
            if (process.IsNull()) return VoidResult.FaultedResult<TResponse>();

            // execute process
            var response = ProcessExecutor.Execute(process);

            return response;
        }

        /// <summary>
        /// Handle the request
        /// </summary>
        /// <returns></returns>
        public TResponse Dispatch<TProcess, T1, TResponse>(T1 obj1)
            where TProcess : IBaseProcess<TResponse>, new()
            where TResponse : VoidResult, new()
        {
            // create instance
            var process = ProcessFactory.Create<TResponse>(typeof(TProcess));
            if (process.IsNull()) return VoidResult.FaultedResult<TResponse>();

            // execute process
            var response = ProcessExecutor.Execute(process, obj1);

            return response;
        }

        /// <summary>
        /// Handle the request
        /// </summary>
        /// <returns></returns>
        public TResponse Dispatch<TProcess, T1, T2, TResponse>(T1 obj1, T2 obj2)
            where TProcess : IBaseProcess<TResponse>, new()
            where TResponse : VoidResult, new()
        {
            // create instance
            var process = ProcessFactory.Create<TResponse>(typeof(TProcess));
            if (process.IsNull()) return VoidResult.FaultedResult<TResponse>();

            // execute process
            var response = ProcessExecutor.Execute(process, obj1, obj2);

            return response;
        }

        /// <summary>
        /// Handle the request
        /// </summary>
        /// <returns></returns>
        public TResponse Dispatch<TProcess, T1, T2, T3, TResponse>(T1 obj1, T2 obj2, T3 obj3)
            where TProcess : IBaseProcess<TResponse>, new()
            where TResponse : VoidResult, new()
        {
            // create instance
            var process = ProcessFactory.Create<TResponse>(typeof(TProcess));
            if (process.IsNull()) return VoidResult.FaultedResult<TResponse>();

            // execute process
            var response = ProcessExecutor.Execute(process, obj1, obj2, obj3);

            return response;
        }


        /// <summary>
        /// Handle the request
        /// </summary>
        /// <returns></returns>
        public TResponse Dispatch<TResponse>(IBaseProcess<TResponse> process)
            where TResponse : VoidResult, new()
        {
            // execute process
            var response = ProcessExecutor.Execute(process);

            return response;
        }

        /// <summary>
        /// Handle the request
        /// </summary>
        /// <returns></returns>
        public TResponse Dispatch<T1, TResponse>(IBaseProcess<TResponse> process, T1 obj1)
            where TResponse : VoidResult, new()
        {
            // execute process
            var response = ProcessExecutor.Execute(process, obj1);

            return response;
        }

        /// <summary>
        /// Handle the request
        /// </summary>
        /// <returns></returns>
        public TResponse Dispatch<T1, T2, TResponse>(IBaseProcess<TResponse> process, T1 obj1, T2 obj2)
            where TResponse : VoidResult, new()
        {
            // execute process
            var response = ProcessExecutor.Execute(process, obj1, obj2);

            return response;
        }

        /// <summary>
        /// Handle the request
        /// </summary>
        /// <returns></returns>
        public TResponse Dispatch<T1, T2, T3, TResponse>(IBaseProcess<TResponse> process, T1 obj1, T2 obj2, T3 obj3)
            where TResponse : VoidResult, new()
        {
            // execute process
            var response = ProcessExecutor.Execute(process, obj1, obj2, obj3);

            return response;
        }

        /// <summary>
        /// Specify process to invoke
        /// </summary>
        /// <typeparam name="TProcess"></typeparam>
        /// <returns></returns>
        public TProcess For<TProcess>()
            where TProcess : class, IBaseProcess<VoidResult>, new()
        {
            var processType = typeof(TProcess);
            var instance = ProcessFactory.Create<VoidResult>(processType);

            return instance as TProcess;
        }
    }
}