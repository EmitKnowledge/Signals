using Signals.Aspects.DI;
using Signals.Aspects.Caching;
using Signals.Core.Processes.Base;
using Signals.Core.Common.Instance;
using Signals.Core.Common.Serialization;
using Signals.Core.Processing.Input.Http;
using Signals.Core.Processing.Results;
using Signals.Core.Web.Behaviour;
using System;
using System.Linq;
using Signals.Core.Common.Reflection;

namespace Signals.Core.Web.Execution.FactoryFilters
{
	/// <summary>
	/// Process factory creation handler
	/// </summary>
	public class IsCachedFactoryFilter : IFactoryFilter
	{
		/// <summary>
		/// Validates instance
		/// </summary>
		/// <typeparam name="TProcess"></typeparam>
		/// <param name="instance"></param>
		/// <param name="type"></param>
		/// <param name="context"></param>
		/// <returns>Is instance valid</returns>
		public MiddlewareResult IsValidInstance<TProcess>(TProcess instance, Type type, IHttpContextWrapper context) where TProcess : IBaseProcess<VoidResult>
		{
			// if cached true, return it right away
			var cacheAttribute = type.GetCachedAttributes<OutputCacheAttribute>().SingleOrDefault();
			if (cacheAttribute == null) return MiddlewareResult.DoNothing;

			this.D("Cache attribute found. Proceed with cache output configuration.");

			if (cacheAttribute.Location != CacheLocation.Server &&
			    cacheAttribute.Location != CacheLocation.ClientAndServer)
			{
				this.D($"Caching -> Location: {cacheAttribute.Location} not supported. Only Server and ClientAndServer location types are supported. Exit filter.");
				return MiddlewareResult.DoNothing;
			}

			var cache = SystemBootstrapper.GetInstance<ICache>();
			if (cache == null)
			{
				this.D("Caching not configured. Exit filter.");
				return MiddlewareResult.DoNothing;
			}

			var key = type.FullName;

			if (!cacheAttribute.VaryByQueryParams.IsNullOrHasZeroElements())
			{
				var queryValues = context
					?.Query
					?.Where(x => cacheAttribute.VaryByQueryParams.Contains(x.Key))
					?.OrderBy(x => x.Key)
					?.Select(x => x.Value)
					?.SerializeJson();

				if (!queryValues.IsNullOrEmpty())
					key = $"{type.FullName}+{queryValues}";
			}

			var entry = cache.Get(key);
			this.D($"Retrieved value for -> Key: {key} -> Has Value: {!entry.IsNull()}.");

			if (!entry.IsNull())
			{
				var webMediator = SystemBootstrapper.GetInstance<WebMediator>();
				var httpContext = SystemBootstrapper.GetInstance<IHttpContextWrapper>();

				foreach (var executeEvent in webMediator.ResultHandlers)
				{
					var result = executeEvent.HandleAfterExecution(instance, type, (VoidResult)entry.Value, httpContext);
					if (result != MiddlewareResult.DoNothing)
					{
						return result;
					}
				}
			}

			this.D("Exit Filter.");

			return MiddlewareResult.DoNothing;
		}
	}
}
