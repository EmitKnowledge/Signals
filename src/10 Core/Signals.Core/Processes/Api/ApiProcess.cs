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
            set { (value as ApiProcessContext).SetProcess(this); _context = value; }
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
    public abstract class ApiProcess<TRequest, TResponse> : BusinessProcess<TRequest, TResponse>,
        IApiProcess
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
            set { (value as ApiProcessContext).SetProcess(this); _context = value; }
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
                obj?.Sanitize(new HtmlSanitizer());
                return Execute(obj);
            }

            throw new ArgumentException("Input is empty");
        }
    }
}