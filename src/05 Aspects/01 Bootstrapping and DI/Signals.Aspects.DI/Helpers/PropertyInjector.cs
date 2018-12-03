using Signals.Aspects.DI.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Signals.Aspects.DI.Helpers
{
    /// <summary>
    /// Helper for injecting properties with <see cref="ImportAttribute"/>
    /// </summary>
    public class PropertyInjector
    {
        private HashSet<Type> _set;

        /// <summary>
        /// Inject all properties and fields annotated with <see cref="ImportAttribute"/> 
        /// </summary>
        /// <param name="serviceContainer"></param>
        /// <param name="obj"></param>
        public void Inject(object obj)
        {
            _set = new HashSet<Type>();
            InjectInternal(obj);
            _set = null;
        }

        /// <summary>
        /// Inject all properties and fields annotated with <see cref="ImportAttribute"/> 
        /// </summary>
        /// <param name="obj"></param>
        private void InjectInternal(object obj)
        {
            try
            {
                if (obj == null) return;
                if (_set.Contains(obj.GetType())) return;
                if (obj.GetType().Assembly.FullName.StartsWith("System")) return;
                if (obj.GetType().Assembly.FullName.StartsWith("Microsoft")) return;
                _set.Add(obj.GetType());

                // public and private fields
                var fields = obj.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

                // public and private porperties
                var properties = obj.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

                // annotated properties with @ImportAttribute
                var injectables = properties.Where(x => x.GetCustomAttributes(typeof(ImportAttribute), true).Any());

                foreach (var injectable in injectables)
                {
                    var instance = SystemBootstrapper.GetInstance(injectable.PropertyType);

                    injectable.SetValue(obj, instance);
                }

                // Recursively inject instances of child properties to all properties
                foreach (var property in properties.Where(x => !x.PropertyType.IsSealed))
                {
                    InjectInternal(property.GetValue(obj));
                }

                // Recursively inject instances of child properties to all fields
                foreach (var field in fields.Where(x => !x.FieldType.IsSealed))
                {
                    InjectInternal(field.GetValue(obj));
                }
            }
            catch
            {
                //TODO: this exception should not go silent
            }
        }
    }
}
