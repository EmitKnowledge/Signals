using Autofac.Core;
using Signals.Aspects.DI.Attributes;
using System.Linq;
using System.Reflection;

namespace Signals.Aspects.DI.Autofac
{
    /// <summary>
    /// Autofac property selection rule definition
    /// </summary>
    public class PropertySelector : IPropertySelector
    {
        /// <summary>
        /// Define rule to inject all properties with @ImportAttribute
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public bool InjectProperty(PropertyInfo propertyInfo, object instance)
        {
            return propertyInfo.GetCustomAttributes<ImportAttribute>().Any();
        }
    }
}
