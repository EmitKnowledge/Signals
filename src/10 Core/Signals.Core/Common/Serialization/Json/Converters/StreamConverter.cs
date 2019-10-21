using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Signals.Core.Common.Serialization.Json.Converters
{
    /// <summary>
    /// Custom JSON.NET Serializer/Deserializer for Streams
    /// </summary>
    public class StreamConverter : JsonConverter<Stream>
    {
        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            if (typeof(Stream).IsAssignableFrom(objectType))
                return true;
            return false;
        }

        /// <summary>
        /// Read value
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override Stream Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return null;
        }

        /// <summary>
        /// Write value
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        public override void Write(Utf8JsonWriter writer, Stream value, JsonSerializerOptions options)
        {
            writer.WriteStringValue("");
        }
    }
}
