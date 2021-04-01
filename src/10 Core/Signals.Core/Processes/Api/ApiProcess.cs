using Ganss.XSS;
using Signals.Aspects.DI.Attributes;
using Signals.Core.Common.Serialization;
using Signals.Core.Processes.Base;
using Signals.Core.Processes.Business;
using Signals.Core.Processes.Distributed;
using Signals.Core.Processing.Input;
using Signals.Core.Processing.Results;
using System;

namespace Signals.Core.Processes.Api
{
    /// <summary>
    /// Represents an api process
    /// </summary>
    internal interface IApiProcess { }

    /// <summary>
    /// Represents an api process
    /// </summary>
    public abstract class ApiProcess<TResponse> : BusinessProcess<TResponse>,
        IApiProcess
        where TResponse : VoidResult, new()
    {
        /// <summary>
        /// Api process context
        /// </summary>
        [Import]
        protected new virtual IApiProcessContext Context
        {
            get => _context;
            set
            {
                (value as ApiProcessContext)?.SetProcess(this);
                _context = value;
            }
        }
        private IApiProcessContext _context;

        /// <summary>
        /// Base process context upcasted from Api process context
        /// </summary>
        internal override IBaseProcessContext BaseContext => Context;
    }

    /// <summary>
    /// Represents an api process
    /// </summary>
    public abstract class ApiProcess<TRequest, TResponse> : BusinessProcess<TRequest, TResponse>, IApiProcess
        where TRequest : IDtoData
        where TResponse : VoidResult, new()
    {
        /// <summary>
        /// Api process context
        /// </summary>
        [Import]
        protected new virtual IApiProcessContext Context
        {
            get => _context;
            set
            {
                (value as ApiProcessContext)?.SetProcess(this);
                _context = value;
            }
        }
        private IApiProcessContext _context;

        /// <summary>
        /// Base process context upcasted from Api process context
        /// </summary>
        internal override IBaseProcessContext BaseContext => Context;

        /// <summary>
        /// Entry point executed by the factory
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        internal override TResponse ExecuteProcess(params object[] args)
        {
            if (args[0] is string request)
            {
                var obj = request.Deserialize<TRequest>();
                obj?.Sanitize(new HtmlSanitizer());
                return Execute(obj);
            }
            else if (args[0] is TRequest obj)
            {
                obj.Sanitize(new HtmlSanitizer());
                return Execute(obj);
            }

            throw new ArgumentException("Input is empty");
        }
    }

    /// <summary>
    /// Represents API process with an underlying business process
    /// </summary>
    public abstract class ApiBusinessProcess<TBusinessProcess, TResponse> : BaseProcess<TResponse>, IApiProcess
        where TBusinessProcess : BusinessProcess<TResponse>, new()
        where TResponse : VoidResult, new()
    {
        /// <summary>
        /// Maps the business process's response DTO to the API process response DTO
        /// </summary>
        public virtual TResponse MapResponse(TResponse response)
            => response;

        /// <summary>
        /// Execution using base strategy
        /// </summary>
        /// <returns></returns>
        internal virtual TResponse Execute()
            => Context.Mediator.Dispatch<TResponse>(typeof(TBusinessProcess));

        /// <summary>
        /// Entry point executed by the factory
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        internal override TResponse ExecuteProcess(params object[] args)
            => MapResponse(Execute());

        /// <summary>
        /// Api process context
        /// </summary>
        [Import]
        protected virtual IApiProcessContext Context
        {
            get => _context;
            set
            {
                if (value is ApiProcessContext context)
                {
                    context.SetProcess(this);
                    _context = context;
                }
            }
        }
        private IApiProcessContext _context;

        /// <summary>
        /// Base process context upcasted from Api process context
        /// </summary>
        internal override IBaseProcessContext BaseContext => Context;
    }

    /// <summary>
    /// Represents API process with an underlying business process
    /// </summary>
    public abstract class ApiBusinessProcess<TBusinessProcess, TRequest, TResponse> : BaseProcess<TResponse>, IApiProcess
        where TBusinessProcess : BusinessProcess<TRequest, TResponse>, new()
        where TRequest : DtoData<TRequest>, IDtoData
        where TResponse : VoidResult, new()
    {
        /// <summary>
        /// Maps the API process input DTO a the business process's input DTO
        /// </summary>
        public TRequest MapRequest(TRequest request)
            => request.Map();

        /// <summary>
        /// Maps the business process's response DTO to the API process response DTO
        /// </summary>
        public virtual TResponse MapResponse(TResponse response)
            => response;

        /// <summary>
        /// Execution using base strategy
        /// </summary>
        /// <returns></returns>
        internal virtual TResponse Execute(TRequest request)
            => Context.Mediator.Dispatch<TRequest, TResponse>(typeof(TBusinessProcess), request);

        /// <summary>
        /// Entry point executed by the factory
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        internal override TResponse ExecuteProcess(params object[] args)
        {
            if (args[0] is string request)
            {
                var obj = request.Deserialize<TRequest>();
                obj?.Sanitize(new HtmlSanitizer());
                return MapResponse(Execute(MapRequest(obj)));
            }
            else if (args[0] is TRequest obj)
            {
                obj.Sanitize(new HtmlSanitizer());
                return MapResponse(Execute(MapRequest(obj)));
            }
            else
            {
                throw new ArgumentException("Input is empty");
            }
        }

        /// <summary>
        /// Api process context
        /// </summary>
        [Import]
        protected virtual IApiProcessContext Context
        {
            get => _context;
            set
            {
                if (value is ApiProcessContext context)
                {
                    context.SetProcess(this);
                    _context = context;
                }
            }
        }
        private IApiProcessContext _context;

        /// <summary>
        /// Base process context upcasted from Api process context
        /// </summary>
        internal override IBaseProcessContext BaseContext => Context;
    }

    /// <summary>
    /// Represents API process with an underlying distributed process
    /// </summary>
    public abstract class ApiDistributedProcess<TDistributedProcess, TTransientData, TResponse> : BaseProcess<TResponse>, IApiProcess
        where TDistributedProcess : DistributedProcess<TTransientData, TResponse>, new()
        where TTransientData : ITransientData
        where TResponse : VoidResult, new()
    {
        /// <summary>
        /// Maps the distributed process's response DTO to the API process response DTO
        /// </summary>
        public virtual TResponse MapResponse(TResponse response)
            => response;

        /// <summary>
        /// Execution using base strategy
        /// </summary>
        /// <returns></returns>
        internal virtual TResponse Execute()
        {
            //var distributedProcess = Continue<TDistributedProcess>();

            //if (distributedProcess != null)
            //{
            //    return Context.Mediator.ProcessExecutor.ExecuteBackground(distributedProcess);
            //}

            //return default(TResponse);

            return Context.Mediator.Dispatch<TResponse>(typeof(TDistributedProcess));
        }

        /// <summary>
        /// Entry point executed by the factory
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        internal override TResponse ExecuteProcess(params object[] args) 
            => MapResponse(Execute());

        /// <summary>
        /// Api process context
        /// </summary>
        [Import]
        protected virtual IApiProcessContext Context
        {
            get => _context;
            set
            {
                if (value is ApiProcessContext context)
                {
                    context.SetProcess(this);
                    _context = context;
                }
            }
        }
        private IApiProcessContext _context;

        /// <summary>
        /// Base process context upcasted from Api process context
        /// </summary>
        internal override IBaseProcessContext BaseContext => Context;
    }

    /// <summary>
    /// Represents API process with an underlying distributed process
    /// </summary>
    public abstract class ApiDistributedProcess<TDistributedProcess, TTransientData, TRequest, TResponse> : BaseProcess<TResponse>, IApiProcess
        where TDistributedProcess : DistributedProcess<TTransientData, TRequest, TResponse>, new()
        where TTransientData : ITransientData
        where TRequest : DtoData<TRequest>, IDtoData
        where TResponse : VoidResult, new()
    {
        /// <summary>
        /// Maps the API process input DTO a the distributed process's input DTO
        /// </summary>
        public virtual TRequest MapRequest(TRequest request) 
            => request.Map();

        /// <summary>
        /// Maps the distributed process's response DTO to the API process response DTO
        /// </summary>
        public virtual TResponse MapResponse(TResponse response) 
            => response;

        /// <summary>
        /// Execution using base strategy
        /// </summary>
        /// <returns></returns>
        internal virtual TResponse Execute(TRequest request)
            => Context.Mediator.Dispatch<TRequest, TResponse>(typeof(TDistributedProcess), request);

        /// <summary>
        /// Entry point executed by the factory
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        internal override TResponse ExecuteProcess(params object[] args)
        {
            if (args[0] is string request)
            {
                var obj = request.Deserialize<TRequest>();
                obj?.Sanitize(new HtmlSanitizer());
                return MapResponse(Execute(MapRequest(obj)));
            }
            else if (args[0] is TRequest obj)
            {
                obj.Sanitize(new HtmlSanitizer());
                return MapResponse(Execute(MapRequest(obj)));
            }
            else
            {
                throw new ArgumentException("Input is empty");
            }
        }

        /// <summary>
        /// Api process context
        /// </summary>
        [Import]
        protected virtual IApiProcessContext Context
        {
            get => _context;
            set
            {
                if (value is ApiProcessContext context)
                {
                    context.SetProcess(this);
                    _context = context;
                }
            }
        }
        private IApiProcessContext _context;

        /// <summary>
        /// Base process context upcasted from Api process context
        /// </summary>
        internal override IBaseProcessContext BaseContext => Context;
    }
}