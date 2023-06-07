using Signals.Core.Processes.Base;
using Signals.Core.Processing.Input.Http;
using Signals.Core.Processing.Results;
using System;
using Signals.Core.Web.Helpers;

namespace Signals.Core.Web.Execution.ExecutionHandlers
{
	/// <summary>
	/// Result handler that adds http response headers based on response content tpye
	/// </summary>
	public class ContentTypeHeaderAdderHandler : IResultHandler
	{
		/// <summary>
		/// Handle process result
		/// </summary>
		/// <typeparam name="TProcess"></typeparam>
		/// <param name="process"></param>
		/// <param name="type"></param>
		/// <param name="response"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public MiddlewareResult HandleAfterExecution<TProcess>(TProcess process, Type type, VoidResult response, IHttpContextWrapper context) where TProcess : IBaseProcess<VoidResult>
		{
			var contentType = type.HttpContentType(response);
			context.Headers.AddToResponse("Content-Type", contentType);

			return MiddlewareResult.DoNothing;
		}
	}
}