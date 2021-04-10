using Newtonsoft.Json;
using NodaTime;
using System;

namespace Signals.Core.Common.Serialization.Json.Converters
{
    /// <summary>
    /// Custom JSON.NET Serializer/Deserializer for Noda Instant
    /// </summary>
    public class NodaInstantConverter : JsonConverter
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
            if (typeof(Instant).IsAssignableFrom(objectType))
                return true;
            return false;
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader"/> to read from.</param><param name="objectType">Type of the object.</param><param name="existingValue">The existing value of object being read.</param><param name="serializer">The calling serializer.</param>
        /// <returns>
        /// The object value.
        /// </returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.Integer ||
                reader.Value == null)
            {
                return null;
            }

            var ticks = (long)reader.Value;
            return Instant.FromUnixTimeTicks(ticks);
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter"/> to write to.</param><param name="value">The value.</param><param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            if (value is Instant? || value is Instant)
            {
                if (value == null)
                {
                    writer.WriteValue(new Instant?());
                }
                else
                {
                    writer.WriteValue(((Instant)value).ToUnixTimeTicks());
                }
            }
        }
    }
}