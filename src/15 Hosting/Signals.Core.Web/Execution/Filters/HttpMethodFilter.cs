using System;
using System.Collections.Concurrent;
using System.Linq;
using Signals.Core.Common.Reflection;
using Signals.Core.Processes.Api;
using Signals.Core.Processing.Input.Http;

namespace Signals.Core.Web.Execution.Filters
{
	/// <summary>
	/// Process type filter
	/// </summary>
	public class HttpMethodFilter : IFilter
	{
		/// <summary>
		/// Cache all processes that has the Signals API Attribute types that match
		/// </summary>
		private static readonly ConcurrentDictionary<string, bool> TypeApiAttributeRegistry = new ConcurrentDictionary<string, bool>();

		/// <summary>
		/// Check if the process type is correct based on the request
		/// </summary>
		/// <param name="type"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public bool IsCorrectProcessType(Type type, IHttpContextWrapper context)
		{
			if (context.HttpMethod == SignalsApiMethod.OPTIONS || context.HttpMethod == SignalsApiMethod.HEAD) return true;

			var attributes = type.GetCachedAttributes<SignalsApiAttribute>();
			if (!attributes.Any()) return true;
			
			var correctMethod = false;
			foreach (var attribute in attributes)
			{
				correctMethod |= attribute.HttpMethod == SignalsApiMethod.ANY || attribute.HttpMethod == context.HttpMethod;
			}

			if (correctMethod)
			{
				this.D($"HTTP -> Method: {context.HttpMethod} match for -> Type: {type.FullName}. Exit Filter.");
			}

			return correctMethod;
		}
	}
}
