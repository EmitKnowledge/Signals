using Signals.Aspects.DI.Attributes;
using System;
using System.Collections;
using System.Collections.Concurrent;
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
        private static ConcurrentDictionary<Type, PropertyInfo[]> _injectables { get; } = new ConcurrentDictionary<Type, PropertyInfo[]>();
        private static ConcurrentDictionary<Type, PropertyInfo[]> _properties { get; } = new ConcurrentDictionary<Type, PropertyInfo[]>();
        private static ConcurrentDictionary<Type, FieldInfo[]> _fields { get; } = new ConcurrentDictionary<Type, FieldInfo[]>();

        /// <summary>
        /// Inject all properties and fields annotated with <see cref="ImportAttribute"/> 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="recursive"></param>
        public void Inject(object obj, bool recursive)
        {
            InjectInternal(obj, recursive);
        }

        /// <summary>
        /// Inject all properties and fields annotated with <see cref="ImportAttribute"/> 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="recursive"></param>
        private void InjectInternal(object obj, bool recursive)
        {
            if (obj == null) return;
            if (obj is IEnumerable) return;

            var type = obj.GetType();
            var assemblyName = type.Assembly.FullName;

            if (assemblyName.StartsWith("System")) return;
            if (assemblyName.StartsWith("Microsoft")) return;

            // public and private properties and fields
            var injectables = GetInjectables(type);

            foreach (var injectable in injectables)
            {
                var instance = SystemBootstrapper.GetInstance(injectable.PropertyType);

                if (instance != null)
                    injectable.SetValue(obj, instance);
            }

            if (recursive)
            {
                var fields = GetFields(type);
                var properties = GetProperties(type);

                // Recursively inject instances of child properties to all properties
                foreach (var property in properties)
                {
                    InjectInternal(property.GetValue(obj), recursive);
                }

                // Recursively inject instances of child properties to all fields
                foreach (var field in fields)
                {
                    InjectInternal(field.GetValue(obj), recursive);
                }
            }
        }

        /// <summary>
        /// Get type injectables
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private PropertyInfo[] GetInjectables(Type type)
        {
            if (!_injectables.ContainsKey(type))
            {
                var injectables = type
                    .GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                    .Where(x => x.GetCustomAttributes(typeof(ImportAttribute), true).Any())
                    .ToArray();

                return _injectables.GetOrAdd(type, injectables);
            }

            return _injectables[type];
        }

        /// <summary>
        /// Get type proerties
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private PropertyInfo[] GetProperties(Type type)
        {
            if (!_properties.ContainsKey(type))
            {
                var properties = type
                    .GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                    .Where(x => !x.PropertyType.IsSealed)
                    .ToArray();

                return _properties.GetOrAdd(type, properties);
            }

            return _properties[type];
        }

        /// <summary>
        /// Get type fields
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private FieldInfo[] GetFields(Type type)
        {
            if (!_fields.ContainsKey(type))
            {
                var fields = type
                    .GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                    .Where(x => !x.FieldType.IsSealed)
                    .ToArray();

                return _fields.GetOrAdd(type, fields);
            }

            return _fields[type];
        }
    }
}
