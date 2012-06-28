namespace SDammann.Utils.Linq {
    using System;
    using System.Collections.Generic;


    /// <summary>
    ///   Extensions for <see cref="IEnumerable{T}" />
    /// </summary>
    public static class EnumerableExtensions {
        /// <summary>
        ///   Executes the specified <paramref name="action" /> for each item in the specified <paramref name="enumerable" />
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <param name="enumerable"> </param>
        /// <param name="action"> </param>
        public static void ForEach<T> (this IEnumerable<T> enumerable, Action<T> action) {
            foreach (T item in enumerable) {
                action.Invoke(item);
            }
        }
    }
}