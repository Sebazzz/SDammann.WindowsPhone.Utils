namespace SDammann.Utils {
    using System;


    /// <summary>
    ///   Represent a strongly typed version of the <see cref="WeakReference" /> class
    /// </summary>
    /// <typeparam name="T"> </typeparam>
    public struct WeakReference<T> : IDisposable where T : class {
        private WeakReference weakReference;

        /// <summary>
        ///   Gets the target.
        /// </summary>
        public T Target {
            get { return (T) this.weakReference.Target; }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="WeakReference&lt;T&gt;" /> struct.
        /// </summary>
        /// <param name="target"> The target. </param>
        public WeakReference (T target) {
            this.weakReference = new WeakReference(target);
        }


        #region IDisposable Members

        /// <summary>
        ///   Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() {
            this.weakReference = null;
        }

        #endregion
    }
}