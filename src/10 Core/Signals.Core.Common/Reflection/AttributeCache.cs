using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Signals.Core.Common.Reflection
{
    internal static class AttributeCache
    {
        private static ConcurrentDictionary<Type, ConcurrentDictionary<Type, Attribute[]>> Attributes { get; set; }

        /// <summary>
        /// CTOR
        /// </summary>
        static AttributeCache()
        {
            Attributes = new ConcurrentDictionary<Type, ConcurrentDictionary<Type, Attribute[]>>(8, 10_000);
        }

        public static T[] Get<T>(Type type) where T : Attribute
        {
            var queryCache = Attributes.GetOrAdd(type, (type) => new ConcurrentDictionary<Type, Attribute[]>(8, 5));
            return (T[])queryCache.GetOrAdd(typeof(T), (attributeType) =>
            {
                return type
                    .GetCustomAttributes(true)
                    .Where(x => x.GetType() == attributeType || x.GetType().IsSubclassOf(attributeType))
                    .Cast<T>()
                    .ToArray();
            });
        }

        public static T[] GetCachedAttributes<T>(this Type type) where T : Attribute
        {
            return Get<T>(type);
        }
    }
}
