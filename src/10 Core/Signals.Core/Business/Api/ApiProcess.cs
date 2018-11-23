﻿using Newtonsoft.Json;
using Signals.Core.Common.Serialization;
using Signals.Core.Business.Base;
using Signals.Core.Business.Business;
using Signals.Core.Processing.Input;
using Signals.Core.Processing.Results;
using System.IO;
using Ganss.XSS;

namespace Signals.Core.Business.Api
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
        protected new virtual ApiProcessContext Context { get; set; }

        /// <summary>
        /// Base process context upcasted from Api process context
        /// </summary>
        internal override BaseProcessContext BaseContext => Context;

        /// <summary>
        /// CTOR
        /// </summary>
        protected ApiProcess()
        {
            Context = new ApiProcessContext();
        }
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
        protected new virtual ApiProcessContext Context { get; set; }

        /// <summary>
        /// Base process context upcasted from Api process context
        /// </summary>
        internal override BaseProcessContext BaseContext => Context;

        /// <summary>
        /// CTOR
        /// </summary>
        protected ApiProcess()
        {
            Context = new ApiProcessContext();
        }

        /// <summary>
        /// Entry point executed by the factory
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        internal override TResponse ExecuteProcess(params object[] args)
        {
            var obj = (args[0] as string).Deserialize<TRequest>();
            obj?.Sanitize(new HtmlSanitizer());
            return Execute(obj);
        }
    }
}