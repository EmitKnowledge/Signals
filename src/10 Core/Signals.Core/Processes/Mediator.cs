using Signals.Aspects.DI.Attributes;
using Signals.Core.Processes.Base;
using Signals.Core.Common.Instance;
using Signals.Core.Processing.Execution;
using Signals.Core.Processing.Results;

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
        [Import] private IProcessFactory ProcessFactory { get; set; }

        /// <summary>
        /// Process executor
        /// </summary>
        [Import] private IProcessExecutor ProcessExecutor { get; set; }

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
            var response = ProcessExecutor.Execute<TResponse>(process);

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
            where TProcess : BaseProcess<TResponse>, new()
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
            where TProcess : BaseProcess<TResponse>, new()
            where TResponse : VoidResult, new()
        {
            // create instance
            var process = ProcessFactory.Create<TResponse>(typeof(TProcess));
            if (process.IsNull()) return VoidResult.FaultedResult<TResponse>();

            // execute process
            var response = ProcessExecutor.Execute(process, obj1, obj2, obj3);

            return response;
        }
    }
}