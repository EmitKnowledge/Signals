using Signals.Aspects.DI.Attributes;
using SimpleInjector;
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
        private readonly Container container;

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="container"></param>
        public ImportPropertySelectionBehavior(Container container)
        {
            this.container = container;
        }

        /// <summary>
        /// Define rule to inject all properties with @ImportAttribute
        /// </summary>
        /// <param name="implementationType"></param>
        /// <param name="prop"></param>
        /// <returns></returns>
        public bool SelectProperty(Type implementationType, PropertyInfo prop)
        {
            // cannot set value
            if (prop.SetMethod == null) return false;

            // has no import attribute
            var hasAttributes = prop.GetCustomAttributes<ImportAttribute>().Any();
            if (!hasAttributes) return false;

            // has no registration
            var hasRegistration = true;
            try
            {
                var registration = container.GetRegistration(prop.PropertyType);
                hasRegistration = registration != null;
            }
            catch
            {
                hasRegistration = false;
            }

            return hasRegistration;
        }
    }
}
