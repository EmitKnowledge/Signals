using Signals.Aspects.DI;
using Signals.Core.Common.Instance;
using Signals.Core.Common.Serialization.Json.Converters;
using System;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Signals.Core.Common.Serialization.Json
{
    /// <summary>
    /// JSON serializer
    /// </summary>
    public class JsonSerializer : ISerializer
    {
        /// <summary>
        /// Deserialize string to object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public T Deserialize<T>(string json)
        {
            return System.Text.Json.JsonSerializer.Deserialize<T>(json);
        }

        /// <summary>
        /// Deserialize string to object
        /// </summary>
        /// <param name="json"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public object Deserialize(string json, Type type)
        {
            return System.Text.Json.JsonSerializer.Deserialize(json, type);
        }

        /// <summary>
        /// Serialize string to object
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public string Serialize(object instance)
        {
            var options = SystemBootstrapper.GetInstance<System.Text.Json.JsonSerializerOptions>();
            if (options.IsNull())
            {
                options = new JsonSerializerOptions();
                options.WriteIndented = false;
                options.PropertyNamingPolicy = new PascalCaseNamingPolicy();
                options.Converters.Add(new UnixDateTimeConverter());
                options.Converters.Add(new NullUnixDateTimeConverter());
                options.Converters.Add(new StreamConverter());
            }

            return System.Text.Json.JsonSerializer.Serialize(instance, options: options);
        }
    }
}
