using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Signals.Core.Common.Serialization.Json
{
    /// <summary>
    /// Serialize to json to lowercase
    /// </summary>
    public class LowercaseJsonSerializer
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            ContractResolver = new LowercaseContractResolver()
        };

        /// <summary>
        /// Serialize object into json
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string SerializeObject(object o)
        {
            return JsonConvert.SerializeObject(o, Formatting.Indented, Settings);
        }

        /// <summary>
        /// Lowercase resolver
        /// </summary>
        public class LowercaseContractResolver : DefaultContractResolver
        {
            /// <summary>
            /// Rrle
            /// </summary>
            /// <param name="propertyName"></param>
            /// <returns></returns>
            protected override string ResolvePropertyName(string propertyName)
            {
                return propertyName.ToLower();
            }
        }
    }
}