using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace Signals.Core.Common.Serialization.Json
{
    /// <summary>
    /// Maps camel cased json fileds to title case except identifiers from a given map of field names
    /// </summary>
    public class CapitalizingPropertyResolver : DefaultContractResolver
    {
        private readonly IDictionary<string, string> _fieldMapping;

        public CapitalizingPropertyResolver(IDictionary<string, string> fieldMapping = null)
        {
            this._fieldMapping = fieldMapping;
        }

        protected override JsonProperty CreateProperty(System.Reflection.MemberInfo member, Newtonsoft.Json.MemberSerialization memberSerialization)
        {
            JsonProperty result = base.CreateProperty(member, memberSerialization);
            string mappedName = null;
            if (_fieldMapping != null)
            {
                _fieldMapping.TryGetValue(
                    (member.DeclaringType != null ? (member.DeclaringType.Name + ".") : "") + member.Name,
                    out mappedName);
            }
            result.PropertyName = mappedName ?? result.PropertyName.Substring(0, 1).ToLower() + result.PropertyName.Substring(1);
            return result;
        }
    };
}