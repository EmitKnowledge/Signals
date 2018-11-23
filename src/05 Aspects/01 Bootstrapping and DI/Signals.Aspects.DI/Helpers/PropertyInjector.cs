using Signals.Aspects.DI.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Signals.Aspects.DI.Helpers
{
    public static class PropertyInjector
    {
        private static HashSet<Type> _set;

        /// <summary>
        /// Inject all properties and fields annotated with <see cref="ImportAttribute"/> 
        /// </summary>
        /// <param name="serviceContainer"></param>
        /// <param name="obj"></param>
        public static void Inject(IServiceContainer serviceContainer, object obj)
        {
            _set = new HashSet<Type>();
	        InjectInternal(serviceContainer, obj);
            _set = null;
        }

        /// <summary>
        /// Inject all properties and fields annotated with <see cref="ImportAttribute"/> 
        /// </summary>
        /// <param name="serviceContainer"></param>
        /// <param name="obj"></param>
        private static void InjectInternal(IServiceContainer serviceContainer, object obj)
        {
            try
            {
                if (obj == null) throw new NullReferenceException();
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
                    var instance = serviceContainer.GetInstance(injectable.PropertyType);

                    injectable.SetValue(obj, instance);
                }

                // Recursively inject instances of child properties to all properties
                foreach (var property in properties.Where(x => !x.PropertyType.IsSealed))
                {
	                InjectInternal(serviceContainer, property.GetValue(obj));
                }

                // Recursively inject instances of child properties to all fields
                foreach (var field in fields.Where(x => !x.FieldType.IsSealed))
                {
	                InjectInternal(serviceContainer, field.GetValue(obj));
                }
            }
	        catch
	        {
				//TODO: this exception should not go silent
	        }
        }
    }
}
