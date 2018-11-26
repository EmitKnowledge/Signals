using System.Text.RegularExpressions;

namespace Signals.Aspects.Localization.InMemory.Helpers
{
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
            if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(pattern)) return false;
            return Regex.IsMatch(value, pattern);
        }

        /// <summary>
        /// Return all regex group matches
        /// </summary>
        /// <param name="value"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static Match GetMatch(this string value, string pattern)
            => string.IsNullOrEmpty(value) ? Match.Empty : new Regex(pattern).Match(value);
    }
}
