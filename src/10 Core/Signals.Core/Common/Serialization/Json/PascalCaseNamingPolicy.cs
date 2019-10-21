using System.Text.Json;

namespace Signals.Core.Common.Serialization.Json
{
    /// <summary>
    /// Maps camel cased json fileds to title case except identifiers from a given map of field names
    /// </summary>
    public class PascalCaseNamingPolicy : JsonNamingPolicy
    {
        /// <summary>
        /// Convert property name to pascal case
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override string ConvertName(string name)
        {
            if (name == null) return null;
            if (name.Length == 0) return string.Empty;

            return name.Insert(0, char.ToUpperInvariant(name[0]).ToString()).Remove(1, 1);
        }
    };
}