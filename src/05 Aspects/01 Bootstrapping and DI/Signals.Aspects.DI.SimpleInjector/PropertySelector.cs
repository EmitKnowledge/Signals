using Signals.Aspects.DI.Attributes;
using SimpleInjector.Advanced;
using System;
using System.Linq;
using System.Reflection;

namespace Signals.Aspects.DI.SimpleInjector
{
    /// <summary>
    /// SimpleInjector property selection rule definition
    /// </summary>
    public class ImportPropertySelectionBehavior : IPropertySelectionBehavior
    {
        /// <summary>
        /// Define rule to inject all properties with @ImportAttribute
        /// </summary>
        /// <param name="implementationType"></param>
        /// <param name="prop"></param>
        /// <returns></returns>
        public bool SelectProperty(Type implementationType, PropertyInfo prop)
        {
            return prop.GetCustomAttributes<ImportAttribute>().Any();
        }
    }
}
