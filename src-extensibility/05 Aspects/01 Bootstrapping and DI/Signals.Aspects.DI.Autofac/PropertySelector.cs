using Autofac.Core;
using Signals.Aspects.DI.Attributes;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace Signals.Aspects.DI.Autofac
{
    /// <summary>
    /// Autofac property selection rule definition
    /// </summary>
    public class PropertySelector : IPropertySelector
    {
        private static ConcurrentDictionary<PropertyInfo, bool> ShouldInject = new ConcurrentDictionary<PropertyInfo, bool>();

        /// <summary>
        /// Define rule to inject all properties with @ImportAttribute
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public bool InjectProperty(PropertyInfo propertyInfo, object instance)
        {
            if (ShouldInject.ContainsKey(propertyInfo)) return ShouldInject[propertyInfo];

            var shouldInject = propertyInfo.GetCustomAttributes<ImportAttribute>().Any();
            ShouldInject.TryAdd(propertyInfo, shouldInject);
            return shouldInject;
        }
    }
}
