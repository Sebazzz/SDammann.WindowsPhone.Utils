namespace SDammann.Utils.IO {
    using System.Collections;
    using System.IO.IsolatedStorage;


    /// <summary>
    /// Represents <see cref="IsolatedStorageSettings"/>
    /// </summary>
    public interface IIsolatedStorageSettings {
        /// <summary>
        /// Gets a value for the specified key.
        /// </summary>
        /// <returns>
        /// true if the specified key is found; otherwise, false.
        /// </returns>
        /// <param name="key">The key of the value to get.</param><param name="value">When this method returns, the value associated with the specified key if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter is passed uninitialized.</param><typeparam name="T">The <see cref="T:System.Type"/> of the <paramref name="value"/> parameter.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception>
        bool TryGetValue<T>(string key, out T value);

        /// <summary>
        /// Returns an <see cref="T:System.Collections.IDictionaryEnumerator"/> object for the <see cref="T:System.Collections.IDictionary"/> object.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IDictionaryEnumerator"/> object for the <see cref="T:System.Collections.IDictionary"/> object.
        /// </returns>
        IDictionaryEnumerator GetEnumerator();

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <returns>
        /// The value associated with the specified key. If the specified key is not found, a get operation throws a <see cref="T:System.Collections.Generic.KeyNotFoundException"/>, and a set operation creates a new element that has the specified key.
        /// </returns>
        /// <param name="key">The key of the item to get or set.</param>
        object this [string key] { get; set; }

        /// <summary>
        /// Removes the entry with the specified key.
        /// </summary>
        /// <returns>
        /// true if the specified key was removed; otherwise, false.
        /// </returns>
        /// <param name="key">The key for the entry to be deleted.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception>
        bool Remove(string key);

        /// <summary>
        /// Determines if the application settings dictionary contains the specified key.
        /// </summary>
        /// <returns>
        /// true if the dictionary contains the specified key; otherwise, false.
        /// </returns>
        /// <param name="key">The key for the entry to be located.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception>
        bool Contains(string key);

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only. </exception>
        void Clear();

        /// <summary>
        /// Saves data written to the current <see cref="T:System.IO.IsolatedStorage.IsolatedStorageSettings"/> object.
        /// </summary>
        /// <exception cref="T:System.IO.IsolatedStorage.IsolatedStorageException">The <see cref="T:System.IO.IsolatedStorage.IsolatedStorageFile"/> does not have enough available free space.</exception>
        void Save();

        /// <summary>
        /// Gets a collection that contains the keys in the dictionary.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.Dictionary`2.KeyCollection"/> that contains the keys in the <see cref="T:System.Collections.Generic.Dictionary`2"/>.
        /// </returns>
        ICollection Keys { get; }
    }
}