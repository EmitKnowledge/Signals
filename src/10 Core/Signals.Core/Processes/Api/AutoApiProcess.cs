using Signals.Aspects.DI.Attributes;
using Signals.Core.Common.Serialization;
using Signals.Core.Processes.Base;
using Signals.Core.Processes.Business;
using Signals.Core.Processing.Input;
using Signals.Core.Processing.Results;
using System;
using Ganss.Xss;

namespace Signals.Core.Processes.Api
{
    /// <summary>
    /// Represents API process with an underlying business process
    /// </summary>
    public abstract class AutoApiProcess<TProcess, TProcessRequest, TApiRequest> : BaseProcess<VoidResult>, IApiProcess
        where TProcess : IBusinessProcess, new()
        where TApiRequest : DtoData<TProcessRequest>, IDtoData
    {
        /// <summary>
        /// Maps the business process response to the API process response
        /// </summary>
        public virtual TProcessRequest MapRequest(TApiRequest request)
            => request.Map();

        /// <summary>
        /// Execution using base strategy
        /// </summary>
        /// <returns></returns>
        internal virtual VoidResult Execute(TProcessRequest request)
            => Context.Mediator.Dispatch<TProcessRequest, VoidResult>(typeof(TProcess), request);

        /// <summary>
        /// Entry point executed by the factory
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        internal override VoidResult ExecuteProcess(params object[] args)
        {
            TApiRequest apiRequest;
            if (args[0] is string request)
            {
                apiRequest = request.Deserialize<TApiRequest>();
            }
            else if (args[0] is TApiRequest req)
            {
                apiRequest = req;
            }
            else
            {
                throw new ArgumentException("Input is empty");
            }

            apiRequest?.Sanitize(new HtmlSanitizer());

            var processRequest = MapRequest(apiRequest);
            var processResponse = Execute(processRequest);

            return processResponse;
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
    /// Represents API process with an underlying business process
    /// </summary>
    public abstract class AutoApiProcess<TProcess, TProcessRequest> : BaseProcess<VoidResult>, IApiProcess
        where TProcess : IBusinessProcess, new()
        where TProcessRequest : IDtoData, new()
    {
        /// <summary>
        /// Execution using base strategy
        /// </summary>
        /// <returns></returns>
        internal virtual VoidResult Execute(TProcessRequest request)
            => Context.Mediator.Dispatch<TProcessRequest, VoidResult>(typeof(TProcess), request);

        /// <summary>
        /// Entry point executed by the factory
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        internal override VoidResult ExecuteProcess(params object[] args)
        {
            TProcessRequest request;
            if (args[0] is string strReq)
            {
                request = strReq.Deserialize<TProcessRequest>();
            }
            else if (args[0] is TProcessRequest objReq)
            {
                request = objReq;
            }
            else
            {
                throw new ArgumentException("Input is empty");
            }

            request?.Sanitize(new HtmlSanitizer());

            var processResponse = Execute(request);

            return processResponse;
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
    /// Represents API process with an underlying business process
    /// </summary>
    public abstract class AutoApiProcess<TProcess> : BaseProcess<VoidResult>, IApiProcess
        where TProcess : IBusinessProcess, new()
    {
        /// <summary>
        /// Execution using base strategy
        /// </summary>
        /// <returns></returns>
        internal virtual VoidResult Execute()
            => Context.Mediator.Dispatch<VoidResult>(typeof(TProcess));

        /// <summary>
        /// Entry point executed by the factory
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        internal override VoidResult ExecuteProcess(params object[] args)
            => Execute();

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