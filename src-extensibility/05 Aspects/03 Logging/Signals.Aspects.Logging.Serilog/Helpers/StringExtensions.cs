using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Signals.Aspects.Logging.Serilog.Helpers
{
    internal static class TextExtensions
    {
        /// <summary>
        /// Convert linebreaks to space in string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string CovertNewlinesToSpace(this string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Replace(System.Environment.NewLine, " ");
        }

        /// <summary>
        /// Return all regex group matches
        /// </summary>
        /// <param name="value"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static Match GetMatch(this string value, string pattern)
        {
            if (string.IsNullOrEmpty(value)) return Match.Empty;
            var regex = new Regex(pattern);
            var regexMatch = regex.Match(value);
            return regexMatch;
        }

        /// <summary>
        /// Extract all messages from exception
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string ExtractMessages(this Exception ex)
        {
            if (ex == null) return null;
            var messages = new List<string>() { ex.Message };
            if (ex.InnerException != null)
            {
                messages.Add(ex.InnerException.Message);
            }

            var message = string.Join(Environment.NewLine, messages);
            return message;
        }

        /// <summary>
        /// Get description of enum
        /// /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

			if (attributes.Length > 0)
			{
				return attributes[0].Description;
			}

	        return value.ToString();
        }
    }
}
