namespace SDammann.Utils.Collections.Generic {
    using System;
    using System.Collections.Generic;
    using Linq;


    /// <summary>
    ///   Extensions for <see cref="IList{T}" />
    /// </summary>
    public static class ListExtensions {
        /// <summary>
        /// Removes all the items specified by <paramref name="itemsToRemove"/> in the <paramref name="list"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list to remove items from.</param>
        /// <param name="itemsToRemove">The items to remove.</param>
        public static void RemoveAll<T> (this IList<T> list, IEnumerable<T> itemsToRemove) {
            if (list == null) {
                throw new ArgumentNullException("list");
            }
            if (itemsToRemove == null) {
                throw new ArgumentNullException("itemsToRemove");
            }

            itemsToRemove.ForEach(item => list.Remove(item));
        }
    }
}