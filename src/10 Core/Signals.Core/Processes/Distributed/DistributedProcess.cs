using Signals.Aspects.DI.Attributes;
using Signals.Core.Common.Serialization;
using Signals.Core.Processes.Base;
using Signals.Core.Processes.Business;
using Signals.Core.Processing.Input;
using Signals.Core.Processing.Results;
using System.Threading;
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
        /// Distributed process context
        /// </summary>
        [Import]
        protected new virtual IDistributedProcessContext Context
        {
            get => _context;
            set { (value as DistributedProcessContext).SetProcess(this); _context = value; }
        }
        private IDistributedProcessContext _context;

        /// <summary>
        /// Base process context upcasted from Api process context
        /// </summary>
        internal override IBaseProcessContext BaseContext => Context;
        
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
            TTransientData transientData = default(TTransientData);

            if (args[0] is string str)
                transientData = str.Deserialize<TTransientData>(SerializationFormat.Json);
            else if (args[0] is TTransientData tran)
                transientData = tran;
            else if (args[0] is object obj)
                transientData = obj.SerializeJson().Deserialize<TTransientData>(SerializationFormat.Json);

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
        internal Task PublishNotificaiton(string name, TTransientData obj)
        {
            var metadata = new DistributedProcessMetadata();
            metadata.EpicId = EpicId;
            metadata.CallerProcessName = CallerProcessName;
            metadata.CultureName = Thread.CurrentThread.CurrentCulture.Name;
            metadata.Payload = obj.SerializeJson();

            return Context.Channel?.Publish(name, metadata);
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
        /// Distributed process context
        /// </summary>
        [Import]
        protected new virtual IDistributedProcessContext Context
        {
            get => _context;
            set { (value as DistributedProcessContext).SetProcess(this); _context = value; }
        }
        private IDistributedProcessContext _context;

        /// <summary>
        /// Base process context upcasted from Api process context
        /// </summary>
        internal override IBaseProcessContext BaseContext => Context;
        
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
            if (args[0] is TRequest obj)
                return Execute(obj);

            var fallbackInput = (args[0] as string).Deserialize<TRequest>();
            return Execute(fallbackInput);
        }

        /// <summary>
        /// Entry point executed by the factory
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        internal VoidResult ExecuteBackgroundProcess(params object[] args)
        {
            TTransientData transientData = default(TTransientData);

            if (args[0] is string str)
                transientData = str.Deserialize<TTransientData>(SerializationFormat.Json);
            else if (args[0] is TTransientData tran)
                transientData = tran;
            else if (args[0] is object obj)
                transientData = obj.SerializeJson().Deserialize<TTransientData>(SerializationFormat.Json);

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
        internal Task PublishNotificaiton(string name, TTransientData obj)
        {
            var metadata = new DistributedProcessMetadata();
            metadata.EpicId = EpicId;
            metadata.CallerProcessName = CallerProcessName;
            metadata.CultureName = Thread.CurrentThread.CurrentCulture.Name;
            metadata.Payload = obj.SerializeJson();

            return Context.Channel?.Publish(name, metadata);
        }
    }
}
