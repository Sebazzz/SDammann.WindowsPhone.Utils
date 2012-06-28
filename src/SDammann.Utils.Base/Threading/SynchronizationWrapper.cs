namespace SDammann.Utils.Threading {
    using System.Diagnostics;
    using System.Threading;


    /// <summary>
    /// Represents a wrapper for object that don't support the <see cref="ISynchronizedObject"/> interface
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class SynchronizationWrapper<T> : ISynchronizedObject where T : class {
        private readonly SynchronizationContext originalContext;
        private readonly T @object;

        /// <summary>
        /// Gets the synchronization context on which this object was created
        /// </summary>
        public SynchronizationContext ObjectSynchronizationContext {
            get { return this.originalContext; }
        }

        /// <summary>
        /// Gets the object.
        /// </summary>
        public T Object {
            [DebuggerStepThrough]
            get { return this.@object; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizationWrapper&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="originalContext">The original context.</param>
        /// <param name="o">The o.</param>
        public SynchronizationWrapper(SynchronizationContext originalContext, T o) {
            this.originalContext = originalContext;
            this.@object = o;
        }
    }
}