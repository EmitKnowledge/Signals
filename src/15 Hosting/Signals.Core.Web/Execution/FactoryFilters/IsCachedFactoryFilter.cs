using Signals.Aspects.DI;
using Signals.Aspects.Caching;
using Signals.Core.Processes.Base;
using Signals.Core.Common.Instance;
using Signals.Core.Common.Serialization;
using Signals.Core.Processing.Input.Http;
using Signals.Core.Processing.Results;
using Signals.Core.Web.Behaviour;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Signals.Core.Web.Execution.FactoryFilters
{
	/// <summary>
	/// Process factory creation handler
	/// </summary>
	public class IsCachedFactoryFilter : IFactoryFilter
	{
		/// <summary>
		/// Cache all process types that match
		/// </summary>
		private static readonly ConcurrentDictionary<string, bool> TypeCachedAttributeRegistry = new ConcurrentDictionary<string, bool>();

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
			var hasCacheAttribute = TypeCachedAttributeRegistry.GetOrAdd(type.FullName, _ =>
			{
				return type
					.GetCustomAttributes(typeof(OutputCacheAttribute), true)
					.Cast<OutputCacheAttribute>()
					.SingleOrDefault() != null;
			});

			if (!hasCacheAttribute) return MiddlewareResult.DoNothing;

			var cacheAttribute = type
				.GetCustomAttributes(typeof(OutputCacheAttribute), true)
				.Cast<OutputCacheAttribute>()
				.SingleOrDefault();

			if (cacheAttribute != null && (cacheAttribute.Location == CacheLocation.Server || cacheAttribute.Location == CacheLocation.ClientAndServer))
			{
				var cache = SystemBootstrapper.GetInstance<ICache>();
				if (cache != null)
				{
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
				}
			}

			return MiddlewareResult.DoNothing;
		}
	}
}
