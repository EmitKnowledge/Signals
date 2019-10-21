using Signals.Core.Common.Dates;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Signals.Core.Common.Serialization.Json.Converters
{
    /// <summary>
    /// Custom JSON.NET Serializer/Deserializer for DateTime - timestamp handling
    /// </summary>
    public class UnixDateTimeConverter : JsonConverter<DateTime>
    {
        /// <summary>
        /// Write value
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteNumberValue((value).Ticks());
            }
        }

        /// <summary>
        /// Read value
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.Number ||
                reader.HasValueSequence == false)
            {
                return default(DateTime);
            }

            var ticks = reader.GetInt64();

            var date = new DateTime(1970, 1, 1);
            date = date.AddTicks(ticks);
            return date;
        }

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime) || objectType == typeof(DateTime?);
        }
    }


    /// <summary>
    /// Custom JSON.NET Serializer/Deserializer for DateTime - timestamp handling
    /// </summary>
    public class NullUnixDateTimeConverter : JsonConverter<DateTime?>
    {
        /// <summary>
        /// Write value
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            if (!value.HasValue)
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteNumberValue((value.Value).Ticks());
            }
        }

        /// <summary>
        /// Read value
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.Number ||
                reader.HasValueSequence == false)
            {
                return null;
            }

            var ticks = reader.GetInt64();

            var date = new DateTime(1970, 1, 1);
            date = date.AddTicks(ticks);
            return date;
        }

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime) || objectType == typeof(DateTime?);
        }
    }
}