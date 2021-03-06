﻿using Autofac.Core;
using Signals.Aspects.DI.Attributes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Signals.Aspects.DI.Autofac
{
    /// <summary>
    /// Autofac property selection rule definition
    /// </summary>
    public class PropertySelector : IPropertySelector
    {
        private static ConcurrentDictionary<(Type, PropertyInfo), bool> ShouldInject = new ConcurrentDictionary<(Type, PropertyInfo), bool>();

        /// <summary>
        /// Define rule to inject all properties with @ImportAttribute
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public bool InjectProperty(PropertyInfo propertyInfo, object instance)
        {
            var key = (propertyInfo.DeclaringType, propertyInfo);

            if (ShouldInject.ContainsKey(key)) return ShouldInject[key];

            var shouldInject = propertyInfo.GetCustomAttributes<ImportAttribute>().Any();
            ShouldInject.TryAdd(key, shouldInject);
            return shouldInject;
        }
    }
}
