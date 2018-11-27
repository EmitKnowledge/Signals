using Signals.Core.Common.Instance;
using System.Text.RegularExpressions;

namespace Signals.Core.Common.Regexes
{
    /// <summary>
    /// Helper for regex
    /// </summary>
    public static class RegexHelper
    {
        /// <summary>
        /// Regex matcher
        /// </summary>
        /// <param name="value"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static bool IsMatch(this string value, string pattern)
        {
            return value.IsMatch(pattern, RegexOptions.None);
        }

        /// <summary>
        /// Regex matcher
        /// </summary>
        /// <param name="value"></param>
        /// <param name="pattern"></param>
        /// <param name="regexOptions"></param>
        /// <returns></returns>
        public static bool IsMatch(this string value, string pattern, RegexOptions regexOptions)
        {
            if (value.IsNullOrEmpty() || pattern.IsNullOrEmpty()) return false;
            return Regex.IsMatch(value, pattern, regexOptions);
        }

        /// <summary>
        /// Return all regex group matches
        /// </summary>
        /// <param name="value"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static MatchCollection GetMatches(this string value, string pattern)
        {
            if (value.IsNullOrEmpty()) return null;
            var regex = new Regex(pattern);
            var regexMatches = regex.Matches(value);
            return regexMatches;
        }

        /// <summary>
        /// Return all regex group matches
        /// </summary>
        /// <param name="value"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static Match GetMatch(this string value, string pattern)
        {
            if (value.IsNullOrEmpty()) return Match.Empty;
            var regex = new Regex(pattern);
            var regexMatch = regex.Match(value);
            return regexMatch;
        }

        /// <summary>
        /// Replace text that match the provided pattern with the provided replacement
        /// (Ignore Case is used for compuling the regex)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="pattern"></param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        public static string Replace(string value, string pattern, string replacement)
        {
            if (value.IsNullOrEmpty()) return value;
            return Regex.Replace(value, pattern, replacement, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Replace text that match the provided pattern with the provided replacement
        /// (Ignore Case is used for compuling the regex)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="pattern"></param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        public static string ReplaceFirst(string value, string pattern, string replacement)
        {
            if (value.IsNullOrEmpty()) return value;
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            return regex.Replace(value, replacement, 1);
        }
    }
}