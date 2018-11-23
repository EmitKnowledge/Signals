using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Signals.Core.Common.Instance
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Create enum instance from a string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stringValue"></param>
        /// <returns></returns>
        public static T FromString<T>(string stringValue) where T : struct
        {
            if (string.IsNullOrEmpty(stringValue)) return default(T);
            var isParsed = Enum.TryParse(stringValue, true, out T value);
            return isParsed ? value : default(T);
        }

        /// <summary>
        /// Create enum instance from an int
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="intValue"></param>
        /// <returns></returns>
        public static T FromInt<T>(int intValue) where T : struct
        {
            var enumInstance = Enum.ToObject(typeof(T), intValue);
            return (T)enumInstance;
        }

        /// <summary>
        /// Checks for valid enum valie (if an enum instance is defined)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool IsDefined<T>(string value) where T : struct
        {
            return Enum.IsDefined(typeof(T), value);
        }

        /// <summary>
        /// Return enum value as int
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static int ToInt(this Enum enumValue)
        {
            return Convert.ToInt32(enumValue);
        }

        /// <summary>
        /// Return description for enum value
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static string GetDescription<T>(T enumValue) where T : struct
        {
            var fi = enumValue.GetType().GetField(enumValue.ToString());

            var attrs = fi?.GetCustomAttributes(typeof(DescriptionAttribute), true);
            if (attrs != null && attrs.Length > 0) return ((DescriptionAttribute)attrs[0]).Description;

            return null;
        }

        /// <summary>
        /// Return the value of the EnumMember attribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static string GetEnumMemberValue<T>(T enumValue) where T : struct, IConvertible
        {
            return typeof(T)
                .GetTypeInfo()
                .DeclaredMembers
                .SingleOrDefault(x => x.Name == enumValue.ToString(CultureInfo.InvariantCulture))
                ?.GetCustomAttribute<EnumMemberAttribute>(false)
                ?.Value;
        }

        /// <summary>
        /// Get enum from description
        /// /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T GetEnumFromDescription<T>(this string value) where T : struct
        {
            var fields = typeof(T).GetFields();

            var field = fields.SingleOrDefault(x => (x.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute)?.Description == value);
            if (field == null) return default(T);

            return FromString<T>(field.Name);
        }
    }
}