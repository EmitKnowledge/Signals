using System;
using System.Net.Http;
using Signals.Core.Processes.Base;
using Signals.Core.Processing.Input.Http;
using Signals.Core.Processing.Results;
using Signals.Core.Web.Helpers;

namespace Signals.Core.Web.Execution.ExecutionHandlers
{
	/// <summary>
	/// Json result handler
	/// </summary>
	public class EmptyResultHandler : IResultHandler
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
			var statusCode = response.ToStatusCode();

			context.PutResponse(new HttpResponseMessage(statusCode));

			// result is always handled, this is result fallback
			return MiddlewareResult.StopExecutionAndStopMiddlewarePipe;
		}
	}
}