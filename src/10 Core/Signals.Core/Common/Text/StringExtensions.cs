using Signals.Core.Common.Instance;
using System;
using System.Linq;

namespace Signals.Core.Common.Text
{
    public static class StringExtensions
    {
        /// <summary>
        /// Get substring safe
        /// </summary>
        /// <param name="input"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string SafeSubstring(this string input, int startIndex, int? length = null)
        {
            // Todo: Check that startIndex + length does not cause an arithmetic overflow
            if (length.HasValue && input.Length >= (startIndex + length.Value))
            {
                return input.Substring(startIndex, length.Value);
            }
            if (input.Length > startIndex)
            {
                return input.Substring(startIndex);
            }
            return string.Empty;
        }

        /// <summary>
        /// Get string left part
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maxNumberOfChars"></param>
        /// <param name="append"></param>
        /// <returns></returns>
        public static string LeftSubstring(this string value, int maxNumberOfChars, string append = "")
        {
            if (value.IsNullOrEmpty()) return value;

            string substring;

            if (value.Length < maxNumberOfChars)
            {
                substring = value;
            }
            else
            {
                substring = value.Substring(0, maxNumberOfChars);
            }

            if (!string.IsNullOrEmpty(append)) substring = substring + append;
            return substring;
        }

        /// <summary>
        /// Truncate text at word
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maxNumberOfChars"></param>
        /// <param name="append"></param>
        /// <returns></returns>
        public static string SafeSubstringAtWord(this string value, int maxNumberOfChars, string append = "")
        {
            if (value.IsNullOrEmpty()) return value;

            string substring;
            if (value.Length < maxNumberOfChars || value.IndexOf(" ", maxNumberOfChars, System.StringComparison.Ordinal) == -1)
            {
                substring = value;
            }
            else
            {
                substring = value.Substring(0, value.IndexOf(" ", maxNumberOfChars, System.StringComparison.Ordinal));
            }

            if (!string.IsNullOrEmpty(append)) substring = substring + append;
            return substring;
        }

        /// <summary>
        /// Convert linebreaks to space in string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string CovertNewlinesToSpace(string value)
        {
            if (value.IsNullOrEmpty()) return value;
            return value.Replace(System.Environment.NewLine, " ");
        }

        /// <summary>
        /// convert string 1st letter to lowercase
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Decapitalize(string str)
        {
            return str.Substring(0, 1).ToLower() + str.Substring(1);
        }

        /// <summary>
        /// Returns the input string with the first character converted to uppercase
        /// </summary>
        public static string FirstLetterToUpperCase(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

        /// <summary>
        /// Return initials from string
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string GetInitials(this string s)
        {
            if (s.IsNullOrEmpty()) return null;
            var args = s.Split(' ', '.').ToList();
            var initials = string.Empty;
            args.ForEach(i =>
            {
                var initial = i[0].ToString().ToUpper().Trim();
                if (!initial.IsNullOrEmpty())
                {
                    initials = initials + initial[0];
                }
            });
            return initials;
        }

        /// <summary>
        /// Convert a string to base 64
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToBase64(this string value)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>
        /// Convert from base 64 to string
        /// </summary>
        /// <param name="base64"></param>
        /// <returns></returns>
        public static string FromBase64(this string base64)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        /// <summary>
        /// Trim string at start
        /// </summary>
        /// <param name="text"></param>
        /// <param name="trim"></param>
        /// <returns></returns>
        public static string TrimStart(this string text, string trim)
        {
            while (text.StartsWith(trim))
            {
                text = text.Substring(trim.Length);
            }

            return text;
        }

        /// <summary>
        /// Trim string at end
        /// </summary>
        /// <param name="text"></param>
        /// <param name="trim"></param>
        /// <returns></returns>
        public static string TrimEnd(this string text, string trim)
        {
            while (text.EndsWith(trim))
            {
                text = text.Substring(0, text.Length - trim.Length);
            }

            return text;
        }

        /// <summary>
        /// Trim string at start and end
        /// </summary>
        /// <param name="text"></param>
        /// <param name="trim"></param>
        /// <returns></returns>
        public static string Trim(this string text, string trim)
        {
            return text.TrimStart(trim).TrimEnd(trim);
        }
    }
}