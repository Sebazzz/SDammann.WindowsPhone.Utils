namespace SDammann.Utils.Collections.Generic {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;


    /// <summary>
    ///   Represents a dictionary that supports prefixes on the key
    /// </summary>
    /// <typeparam name="TValue"> The type of the value. </typeparam>
    public sealed class PrefixDictionary<TValue> : IDictionary<string, TValue> {
        private readonly IDictionary<string, TValue> originalDictionary;
        private readonly string prefix;

        /// <summary>
        ///   Initializes a new instance of the <see cref="PrefixDictionary&lt;TValue&gt;" /> class.
        /// </summary>
        /// <param name="originalDictionary"> The original dictionary. </param>
        /// <param name="prefix"> The prefix. </param>
        public PrefixDictionary(IDictionary<string, TValue> originalDictionary, string prefix) {
            this.originalDictionary = originalDictionary;
            this.prefix = prefix + "^";
        }


        #region IDictionary<string,TValue> Members

        public void Add(string key, TValue value) {
            this.originalDictionary.Add(this.prefix + key, value);
        }

        public void Add(KeyValuePair<string, TValue> item) {
            this.originalDictionary.Add(this.CreatePrefixedKeyValuePair(item));
        }

        public void Clear() {
            this.originalDictionary.Clear();
        }

        public bool Contains(KeyValuePair<string, TValue> item) {
            return this.originalDictionary.Contains(this.CreatePrefixedKeyValuePair(item));
        }

        public bool ContainsKey(string key) {
            return this.originalDictionary.ContainsKey(this.prefix + key);
        }

        public void CopyTo(KeyValuePair<string, TValue>[] array, int arrayIndex) {
            KeyValuePair<string, TValue>[] originalCopy = new KeyValuePair<string, TValue>[array.Length];
            this.originalDictionary.CopyTo(originalCopy, 0);

            int q = 0;
            for (int i = arrayIndex; i < array.Length && q < originalCopy.Length; i++) {
                KeyValuePair<string, TValue> item = originalCopy[q];

                if (item.Key.StartsWith(this.prefix)) {
                    array[i] = this.CreateUnPrefixedKeyValuePair(originalCopy[q]);
                } else {
                    i--;
                }
            }
        }

        public int Count {
            get { return this.originalDictionary.Count(kvp => kvp.Key.StartsWith(this.prefix)); }
        }

        public IEnumerator<KeyValuePair<string, TValue>> GetEnumerator() {
            return new PrefixDictionaryEnumerator(this.originalDictionary, this.prefix);
        }

        public bool IsReadOnly {
            get { return this.originalDictionary.IsReadOnly; }
        }

        public TValue this[string key] {
            get {
                if (!this.ContainsKey(this.prefix + key)) {
                    return default(TValue);
                }

                return this.originalDictionary[this.prefix + key];
            }
            set { this.originalDictionary[this.prefix + key] = value; }
        }

        public ICollection<string> Keys {
            get { return this.originalDictionary.Keys; }
        }

        public bool Remove(string key) {
            return this.originalDictionary.Remove(this.prefix + key);
        }

        public bool Remove(KeyValuePair<string, TValue> item) {
            return this.originalDictionary.Remove(this.CreatePrefixedKeyValuePair(item));
        }

        public bool TryGetValue(string key, out TValue value) {
            return this.originalDictionary.TryGetValue(this.prefix + key, out value);
        }

        public ICollection<TValue> Values {
            get { return this.originalDictionary.Values; }
        }

        #endregion


        private KeyValuePair<string, TValue> CreatePrefixedKeyValuePair(KeyValuePair<string, TValue> item) {
            return new KeyValuePair<string, TValue>(this.prefix + item.Key, item.Value);
        }

        private KeyValuePair<string, TValue> CreateUnPrefixedKeyValuePair(KeyValuePair<string, TValue> item) {
            return new KeyValuePair<string, TValue>(item.Key.Substring(this.prefix.Length), item.Value);
        }


        #region Nested type: PrefixDictionaryEnumerator

        /// <summary>
        ///   Enumerator for the <see cref="PrefixDictionary{TValue}" />
        /// </summary>
        private sealed class PrefixDictionaryEnumerator : IEnumerator<KeyValuePair<string, TValue>> {
            private readonly IDictionary<string, TValue> originalDictionary;
            private readonly string prefix;
            private bool disposed;
            private IEnumerator<KeyValuePair<string, TValue>> originalEnumerator;

            /// <summary>
            ///   Initializes a new instance of the <see cref="PrefixDictionary&lt;TValue&gt;.PrefixDictionaryEnumerator" /> class.
            /// </summary>
            /// <param name="originalDictionary"> The original dictionary. </param>
            /// <param name="prefix"> The prefix. </param>
            public PrefixDictionaryEnumerator(IDictionary<string, TValue> originalDictionary, string prefix) {
                this.originalDictionary = originalDictionary;
                this.prefix = prefix;

                this.originalEnumerator = this.originalDictionary.GetEnumerator();
            }


            #region IEnumerator<KeyValuePair<string,TValue>> Members

            /// <summary>
            ///   Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose() {
                this.Dispose(true);

                GC.SuppressFinalize(this);
            }

            public bool MoveNext() {
                do {
                    bool res = this.originalEnumerator.MoveNext();

                    if (!res) {
                        return false;
                    }
                } while (!this.originalEnumerator.Current.Key.StartsWith(this.prefix));

                return true;
            }

            public void Reset() {
                this.originalEnumerator.Reset();
            }

            object IEnumerator.Current {
                get { return this.Current; }
            }

            public KeyValuePair<string, TValue> Current {
                get { return this.CreateUnPrefixedKeyValuePair(this.originalEnumerator.Current); }
            }

            #endregion


            /// <summary>
            ///   Releases unmanaged resources and performs other cleanup operations before the <see cref="PrefixDictionaryEnumerator" /> is reclaimed by garbage collection.
            /// </summary>
            ~PrefixDictionaryEnumerator() {
                this.Dispose(false);
            }

            /// <summary>
            ///   Releases unmanaged and - optionally - managed resources
            /// </summary>
            /// <param name="disposing"> <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources. </param>
            private void Dispose(bool disposing) {
                if (!this.disposed) {
                    if (disposing) {
                        if (this.originalEnumerator != null) {
                            this.originalEnumerator.Dispose();
                        }
                    }

                    this.originalEnumerator = null;
                    this.disposed = true;
                }
            }

            private KeyValuePair<string, TValue> CreateUnPrefixedKeyValuePair(KeyValuePair<string, TValue> item) {
                return new KeyValuePair<string, TValue>(item.Key.Substring(this.prefix.Length), item.Value);
            }
        }

        #endregion


        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
    }
}