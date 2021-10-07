using Ganss.XSS;
using Signals.Aspects.DI.Attributes;
using Signals.Core.Common.Serialization;
using Signals.Core.Processes.Base;
using Signals.Core.Processes.Business;
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
	        var isValidRequest = true;
	        TRequest request = default(TRequest);
            if (args[0] is string json)
            {
	            request = json.Deserialize<TRequest>();
	            this.D($"Deserialized string request: {json}.");
            }
            else if (args[0] is TRequest obj)
            {
	            request = obj;
            }
            else
            {
	            isValidRequest = false;
	            this.D("Request input is empty.");
            }

            if (isValidRequest)
            {
	            request?.Sanitize(new HtmlSanitizer());
	            this.D("Sanitized request input.");
                
	            return Execute(request);
            }

            throw new ArgumentException("Request input is empty.");
        }
    }
}