using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Signals.Aspects.Bootstrap;
using Signals.Aspects.Caching;
using Signals.Aspects.Caching.Entries;
using Signals.Aspects.Caching.Enums;
using Signals.Core.Business.Base;
using Signals.Core.Common.Instance;
using Signals.Core.Common.Serialization;
using Signals.Core.Processing.Input.Http;
using Signals.Core.Processing.Results;
using Signals.Core.Web.Behaviour;
using Signals.Core.Web.Http;

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
            if (!response.IsFaulted)
            {
                var cacheAttribute = type.GetCustomAttributes(typeof(OutputCacheAttribute), true).Cast<OutputCacheAttribute>().SingleOrDefault();

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

                        if (entry.IsNull())
                        {
                            cache.Set(new CacheEntry(key, response)
                            {
                                ExpirationTime = TimeSpan.FromSeconds(cacheAttribute.Duration),
                                ExpirationPolicy = CacheExpirationPolicy.Absolute
                            });
                        }
                    }
                }

                if (cacheAttribute != null && (cacheAttribute.Location == CacheLocation.Client || cacheAttribute.Location == CacheLocation.ClientAndServer))
                {
                    context.Headers.AddToResponse("Cache-Control", $"max-age={cacheAttribute.Duration}, public");
                }
            }

            return MiddlewareResult.DoNothing;
        }
    }
}
