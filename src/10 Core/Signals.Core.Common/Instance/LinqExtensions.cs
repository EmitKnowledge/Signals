using System;
using System.Collections.Generic;
using System.Linq;

namespace Signals.Core.Common.Instance
{
    /// <summary>
    /// Extensions for LINQ
    /// </summary>
    public static class LinqExtensions
    {
        /// <summary>
        /// Distinct by
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var seenKeys = new HashSet<TKey>();
            return source.Where(element => seenKeys.Add(keySelector(element)));
        }

        /// <summary>
        /// Except by property comparison
        /// </summary>
        /// <typeparam name="TA"></typeparam>
        /// <typeparam name="TB"></typeparam>
        /// <typeparam name="TK"></typeparam>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="selectKeyA"></param>
        /// <param name="selectKeyB"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static IEnumerable<TA> Except<TA, TB, TK>(this IEnumerable<TA> a, IEnumerable<TB> b, Func<TA, TK> selectKeyA, Func<TB, TK> selectKeyB, IEqualityComparer<TK> comparer = null)
        {
            return a.Where(aItem => !b.Select(selectKeyB).Contains(selectKeyA(aItem), comparer));
        }

        /// <summary>
        /// Hierarhchy enumerator
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="nextItem"></param>
        /// <param name="canContinue"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> FromHierarchy<TSource>(
            this TSource source,
            Func<TSource, TSource> nextItem,
            Func<TSource, bool> canContinue)
        {
            for (var current = source; canContinue(current); current = nextItem(current))
            {
                yield return current;
            }
        }

        /// <summary>
        /// Hierarhchy enumerator
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="nextItem"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> FromHierarchy<TSource>(
            this TSource source,
            Func<TSource, TSource> nextItem)
            where TSource : class
        {
            return FromHierarchy(source, nextItem, s => s != null);
        }

        /// <summary>
        /// Shuffle
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> Shuffle<TSource>(this IEnumerable<TSource> source)
        {
            if (source.IsNull() || source.Count() == 0)
                yield break;

            var rand = new Random();
            var indexes = Enumerable.Range(0, source.Count()).ToArray();
            for (int i = source.Count() - 1; i > 0; i--)
            {
                var r = rand.Next(i);
                var temp = indexes[i];
                indexes[i] = indexes[r];
                indexes[r] = temp;
            }

            foreach (var index in indexes)
            {
                yield return source.ElementAt(index);
            }
        }
    }
}