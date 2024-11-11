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
    /// Represents a distributed usecase process
    /// </summary>
    public abstract class DistributedProcess<TTransientData, TResponse> : BusinessProcess<TResponse>,
        IDistributedProcess
        where TResponse : VoidResult, new()
        where TTransientData : ITransientData
    {
        /// <summary>
        /// CTOR
        /// </summary>
        protected DistributedProcess()
        {
            _cancelDistributedExecution = false;
        }

        /// <summary>
        /// Distributed process context
        /// </summary>
        [Import]
        protected new virtual IDistributedProcessContext Context
        {
            get => _context;
            set { (value as DistributedProcessContext)?.SetProcess(this); _context = value; }
        }
        private IDistributedProcessContext _context;
        private bool _cancelDistributedExecution;

        /// <summary>
        /// Base process context upcasted from Distributed process context
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
        /// Don't publish request to the distributed background method @Work()
        /// </summary>
        protected void CancelDistributedExecution()
        {
            _cancelDistributedExecution = true;
            this.D("Message publishing cancelled.");
        }

        /// <summary>
        /// Execution using base strategy
        /// </summary>
        /// <returns></returns>
        internal override TResponse Execute()
        {
            var result = base.Execute();
            if (result.IsFaulted)
            {
	            this.D("Executed -> Execute -> Failed.");
                return result;
            }

            if (!_cancelDistributedExecution)
            {
	            PublishNotification(Name, Map(result));
	            this.D("Executed -> Publish Notification.");
            }

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
            var transientData = default(TTransientData);

            if (args[0] is string str)
            {
	            transientData = str.Deserialize<TTransientData>(SerializationFormat.Json);
	            this.D($"Deserialized transient data: {str}.");
            }
            else if (args[0] is TTransientData tran)
            {
	            transientData = tran;
            }
            else if (args[0] is { } obj)
            {
	            var json = obj.SerializeJson();
	            transientData = json.Deserialize<TTransientData>(SerializationFormat.Json);
	            this.D($"Deserialized transient data from object: {json}.");
            }

            return Work(transientData);
        }

        VoidResult IDistributedProcess.ExecuteBackgroundProcess(params object[] args)
        {
            return ExecuteBackgroundProcess(args);
        }

        /// <summary>
        /// Handler for publishing events to communication channel
        /// </summary>
        /// <param name="name"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal Task PublishNotification(string name, TTransientData obj)
        {
            var metadata = new DistributedProcessMetadata();
            metadata.CorrelationId = CorrelationId;
            metadata.CallerProcessName = CallerProcessName;
            metadata.CultureName = Thread.CurrentThread.CurrentCulture.Name;
            metadata.Payload = obj.SerializeJson();

            var publishResult = Context.Channel?.Publish(name, metadata);
            this.D($"Publishing notification -> Message metadata: {metadata.SerializeJson()}.");
            return publishResult;
        }
    }

    /// <summary>
    /// Represents a usecase
    /// </summary>
    public abstract class DistributedProcess<TTransientData, TRequest, TResponse> : BusinessProcess<TRequest, TResponse>,
        IDistributedProcess
        where TResponse : VoidResult, new()
        where TTransientData : ITransientData
    {
        /// <summary>
        /// CTOR
        /// </summary>
        public DistributedProcess()
        {
            _cancelDistributedExecution = false;
        }

        /// <summary>
        /// Distributed process context
        /// </summary>
        [Import]
        protected new virtual IDistributedProcessContext Context
        {
            get => _context;
            set { (value as DistributedProcessContext)?.SetProcess(this); _context = value; }
        }
        private IDistributedProcessContext _context;
        private bool _cancelDistributedExecution;

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
        /// Don't publish request to the distributed background method @Work()
        /// </summary>
        protected void CancelDistributedExecution()
        {
            _cancelDistributedExecution = true;
            this.D("Message publishing cancelled.");
        }

        /// <summary>
        /// Execution using base strategy
        /// </summary>
        /// <returns></returns>
        internal override TResponse Execute(TRequest request)
        {
            var result = base.Execute(request);
            if (result.IsFaulted)
            {
	            this.D("Executed -> Execute -> Failed.");
                return result;
            }

            if (!_cancelDistributedExecution)
            {
	            PublishNotification(Name, Map(request, result));
	            this.D("Executed -> Publish Notification.");
            }

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
	        {
		        return Execute(obj);
	        }

	        this.D("Input data not found. Proceed with fallback input.");
            var fallbackInput = (args[0] as string).Deserialize<TRequest>();
            this.D($"Deserialized fallback input data: {fallbackInput}.");

            return Execute(fallbackInput);
        }

        /// <summary>
        /// Entry point executed by the factory
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        internal VoidResult ExecuteBackgroundProcess(params object[] args)
        {
            var transientData = default(TTransientData);

            if (args[0] is string str)
            {
	            transientData = str.Deserialize<TTransientData>(SerializationFormat.Json);
	            this.D($"Deserialized transient data: {str}.");
            }
            else if (args[0] is TTransientData tran)
            {
	            transientData = tran;
            }
            else if (args[0] is { } obj)
            {
	            var json = obj.SerializeJson();
	            transientData = json.Deserialize<TTransientData>(SerializationFormat.Json);
	            this.D($"Deserialized transient data from object: {json}.");
            }

            return Work(transientData);
        }

        VoidResult IDistributedProcess.ExecuteBackgroundProcess(params object[] args)
        {
            return ExecuteBackgroundProcess(args);
        }

        /// <summary>
        /// Handler for publishing events to communication channel
        /// </summary>
        /// <param name="name"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal Task PublishNotification(string name, TTransientData obj)
        {
            var metadata = new DistributedProcessMetadata();
            metadata.CorrelationId = CorrelationId;
            metadata.CallerProcessName = CallerProcessName;
            metadata.CultureName = Thread.CurrentThread.CurrentCulture.Name;
            metadata.Payload = obj.SerializeJson();

            var publishResult = Context.Channel?.Publish(name, metadata);
            this.D($"Publishing notification -> Message metadata: {metadata.SerializeJson()}.");
            return publishResult;
        }
    }
}
