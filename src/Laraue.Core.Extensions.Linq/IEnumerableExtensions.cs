using System;
using System.Collections.Generic;
using System.Linq;

namespace Laraue.Core.Extensions
{
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Execute action on each element of collection. If collection is null, do nothing.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="action"></param>
        public static void SafeForEach<T>(this IEnumerable<T> data, Action<T> action)
        {
            foreach (T value in data ?? Enumerable.Empty<T>())
            {
                action(value);
            }
        }

        /// <summary>
        /// Get random element of collection or default value, if it is not exists.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public static T Random<T>(this IEnumerable<T> enumerable)
        {
            var r = new Random();
            var length = enumerable.Count();
            return length > 0 ? enumerable.ElementAt(r.Next(0, length)) : default;
        }
    }
}