using System;
using System.Collections;
using System.Collections.Generic;

namespace Signals.Core.Common.Instance
{
    public static class InstanceExtensions
    {
        /// <summary>
        /// Check if an instance is null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool IsNull<T>(this T instance) where T : class
        {
            return instance == null;
        }

        /// <summary>
        /// Check if an instance is null or has zero elements
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool IsNullOrHasZeroElements<T>(this T instance) where T : class, ICollection
        {
            return IsNull(instance) || instance.Count == 0;
        }

        /// <summary>
        /// Check if an instance is null or has zero elements
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool IsNullOrHasZeroElements<T>(this IList<T> instance)
        {
            return IsNull(instance) || instance.Count == 0;
        }

        /// <summary>
        /// Check if a string is null or empty
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string instance)
        {
            return string.IsNullOrEmpty(instance);
        }

        /// <summary>
        /// Check equality for two strings (Case insensitive)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="otherValue"></param>
        /// <returns></returns>
        public static bool IsEqual(this string value, string otherValue)
        {
            var comparer = StringComparer.OrdinalIgnoreCase;
            return 0 == comparer.Compare(value, otherValue);
        }

        /// <summary>
        /// Check if the value is between the provided range
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="lower"></param>
        /// <param name="upper"></param>
        /// <returns></returns>
        public static bool Between<T>(T value, T lower, T upper) where T : IComparable<T>
        {
            return value.CompareTo(lower) >= 0 && value.CompareTo(upper) <= 0;
        }
    }
}