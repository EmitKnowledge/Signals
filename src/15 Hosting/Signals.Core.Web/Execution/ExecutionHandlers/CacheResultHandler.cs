using System;
using System.Linq;
using Signals.Aspects.DI;
using Signals.Aspects.Caching;
using Signals.Aspects.Caching.Entries;
using Signals.Aspects.Caching.Enums;
using Signals.Core.Processes.Base;
using Signals.Core.Common.Instance;
using Signals.Core.Common.Reflection;
using Signals.Core.Common.Serialization;
using Signals.Core.Processing.Input.Http;
using Signals.Core.Processing.Results;
using Signals.Core.Web.Behaviour;

namespace Signals.Core.Web.Execution.ExecutionHandlers
{
    /// <summary>
    /// Cache result handler
    /// </summary>
    public class CacheResultHandler : IResultHandler
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
	        if (response.IsFaulted)
	        {
		        return MiddlewareResult.DoNothing;
	        }

	        var cacheAttribute = type.GetCachedAttributes<OutputCacheAttribute>().SingleOrDefault();
			if (cacheAttribute == null)
	        {
		        return MiddlewareResult.DoNothing;
			}

            if ((cacheAttribute.Location == CacheLocation.Server || cacheAttribute.Location == CacheLocation.ClientAndServer))
            {
				this.D("Cache attribute found. Proceed with cache output configuration.");
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
		            this.D($"Retrieved value -> Key: {key} -> Has Value: {!entry.IsNull()}.");

					if (entry.IsNull())
		            {
			            cache.Set(new CacheEntry(key, response)
			            {
				            ExpirationTime = TimeSpan.FromSeconds(cacheAttribute.Duration),
				            ExpirationPolicy = CacheExpirationPolicy.Absolute
			            });
			            this.D($"No value has been retrieved -> Key: {key}. New value set.");
					}
	            }
	            else
	            {
		            this.D("Caching not configured.");
				}
            }

            if ((cacheAttribute.Location == CacheLocation.Client || cacheAttribute.Location == CacheLocation.ClientAndServer))
            {
	            context.Headers.AddToResponse("Cache-Control", $"max-age={cacheAttribute.Duration}, public");
	            this.D($"Returned cache response header to response -> Key: Cache-Control -> Value: max-age={cacheAttribute.Duration}, public. Exit Handler.");
			}

            this.D("Exit Handler.");

			return MiddlewareResult.DoNothing;
        }
    }
}
