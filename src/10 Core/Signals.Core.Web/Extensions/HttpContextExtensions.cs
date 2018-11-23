using Newtonsoft.Json;
using Signals.Core.Common.Instance;
using Signals.Core.Common.Serialization;
using Signals.Core.Web.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Signals.Core.Web.Extensions
{
    /// <summary>
    /// Custom http context extensions
    /// </summary>
    public static class HttpContextExtensions
    {
        /// <summary>
        /// Resolve candidate process names based on the request url
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string[] GetCandidateProcessNames(this HttpContextWrapper context)
        {
            var pathParts = context.RawUrl.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            if (pathParts.Count() != 2) return new string[0];
            if (pathParts[0].ToLowerInvariant() != "api") return new string[0];

            var nameRoot = pathParts[1];
            if (nameRoot.IsNullOrEmpty()) return new string[0];

            return new[]
            {
                nameRoot,
                $"{nameRoot}Process",
                $"{nameRoot}ApiProcess",
                $"{nameRoot}DistributedProcess",
            };
        }
    }
}