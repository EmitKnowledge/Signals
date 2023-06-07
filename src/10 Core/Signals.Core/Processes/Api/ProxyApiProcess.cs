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
    public abstract class ProxyApiProcess<TProcess, TProcessRequest, TProcessResponse, TApiRequest, TApiResponse> : BaseProcess<TApiResponse>, IApiProcess
        where TProcess : IBusinessProcess, new()
        where TProcessResponse : VoidResult, new()
        where TApiRequest : DtoData<TProcessRequest>, IDtoData
        where TApiResponse : VoidResult, new()
    {
        /// <summary>
        /// Maps the API process input DTO to the business process input DTO
        /// </summary>
        public virtual TProcessRequest MapRequest(TApiRequest request)
            => request.Map();

        /// <summary>
        /// Maps the business process response to the API process response
        /// </summary>
        public abstract TApiResponse MapResponse(TProcessResponse response);

        /// <summary>
        /// Execution using base strategy
        /// </summary>
        /// <returns></returns>
        internal virtual TProcessResponse Execute(TProcessRequest request)
            => Context.Mediator.Dispatch<TProcessRequest, TProcessResponse>(typeof(TProcess), request);

        /// <summary>
        /// Entry point executed by the factory
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        internal override TApiResponse ExecuteProcess(params object[] args)
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
            var apiResponse = MapResponse(processResponse);

            return apiResponse;
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
    public abstract class ProxyApiProcess<TProcess, TProcessRequest, TProcessResponse, TApiResponse> : BaseProcess<TApiResponse>, IApiProcess
        where TProcess : IBusinessProcess, new()
        where TProcessResponse : VoidResult, new()
        where TProcessRequest : IDtoData
        where TApiResponse : VoidResult, new()
    {
        /// <summary>
        /// Maps the business process response to the API process response
        /// </summary>
        public abstract TApiResponse MapResponse(TProcessResponse response);

        /// <summary>
        /// Execution using base strategy
        /// </summary>
        /// <returns></returns>
        internal virtual TProcessResponse Execute(TProcessRequest request)
            => Context.Mediator.Dispatch<TProcessRequest, TProcessResponse>(typeof(TProcess), request);

        /// <summary>
        /// Entry point executed by the factory
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        internal override TApiResponse ExecuteProcess(params object[] args)
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
            var apiResponse = MapResponse(processResponse);

            return apiResponse;
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
    public abstract class ProxyApiProcess<TProcess, TProcessResponse, TApiResponse> : BaseProcess<TApiResponse>, IApiProcess
        where TProcess : IBusinessProcess, new()
        where TProcessResponse : VoidResult, new()
        where TApiResponse : VoidResult, new()
    {
        /// <summary>
        /// Maps the business process response to the API process response
        /// </summary>
        public abstract TApiResponse MapResponse(TProcessResponse response);

        /// <summary>
        /// Execution using base strategy
        /// </summary>
        /// <returns></returns>
        internal virtual TProcessResponse Execute()
            => Context.Mediator.Dispatch<TProcessResponse>(typeof(TProcess));

        /// <summary>
        /// Entry point executed by the factory
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        internal override TApiResponse ExecuteProcess(params object[] args)
        {
            var processResponse = Execute();
            var apiResponse = MapResponse(processResponse);

            return apiResponse;
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