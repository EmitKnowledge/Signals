using Signals.Core.Common.Serialization;
using System.Collections.Generic;

namespace Signals.Core.Processing.Input.Http.ModelBinding
{
    /// <summary>
    /// Create a IDto instance from the query string content
    /// </summary>
    public class FromQuery : BaseModelBinder
    {
        /// <summary>
        /// Bind the HTTP Context Request data to an object
        /// </summary>
        /// <returns></returns>
        public override object Bind(IHttpContextWrapper httpContext)
        {
            if (httpContext?.Query == null)
            {
	            this.D("Query is null. Exit Model Binder.");
                return null;
            }

            // Json query parameters are serialized in the http context
            // We should deserialize them in order to avoid double serialization
            var query = new Dictionary<string, object>();
            foreach (var keyValuePair in httpContext.Query)
            {
                if (keyValuePair.Value is string && keyValuePair.Value.ToString().StartsWith("{") && keyValuePair.Value.ToString().EndsWith("}"))
                {
                    query.Add(keyValuePair.Key, keyValuePair.Value.ToString().Deserialize<object>(SerializationFormat.Json));
                }
                else if (keyValuePair.Value is List<object>)
                {
                    query.Add(keyValuePair.Key, DeserializeList((List<object>)keyValuePair.Value));
                }
                else
                {
                    query.Add(keyValuePair.Key, keyValuePair.Value);
                }
            }

            var dto = query.SerializeJson();
            this.D($"Query -> Value: {dto}. Exit Model Binder.");

            return dto;
        }

        private object DeserializeList(List<object> values)
        {
            var list = new List<object>();
            foreach (var value in values)
            {
                if (value is string && value.ToString().StartsWith("{") && value.ToString().EndsWith("}"))
                {
                    list.Add(value.ToString().Deserialize<object>(SerializationFormat.Json));
                }
                else if (value is List<object>)
                {
                    list.Add(DeserializeList((List<object>)value));
                }
                else
                {
                    list.Add(value);
                }
            }

            return list;
        }
    }
}
