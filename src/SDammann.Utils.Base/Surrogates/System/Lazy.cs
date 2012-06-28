// ReSharper disable CheckNamespace
namespace System {
    using Diagnostics;

    /// <summary>
    /// Provides support for lazy initialization.
    /// </summary>
    /// <remarks>
    /// Lazy initialization occurs the first time the <see cref="Lazy{T}.Value"/> property is accessed.Use an instance 
    /// of <see cref="Lazy{T}"/> to defer the creation of a large or resource-intensive object or the execution of a 
    /// resource-intensive task, particularly when such creation or execution might not occur during the lifetime of the program.
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public sealed class Lazy<T> {
        private readonly Func<T> valueFactory; 
        private bool isValueCreated;
        private T value;

        /// <summary>
        /// Gets a value that indicates whether a value has been created for this <see cref="Lazy{T}"/> instance.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if a value created; otherwise, <c>false</c>.
        /// </value>
        public bool IsValueCreated {
            [DebuggerStepThrough]
            get { return this.isValueCreated; }
        }

        /// <summary>
        /// Gets the lazily initialized value of the current <see cref="Lazy{T}"/> instance.
        /// </summary>
        public T Value {
            [DebuggerStepThrough]
            get {
                if (!this.isValueCreated) {
                    this.value = valueFactory.Invoke();
                    this.isValueCreated = true;
                }

                return this.value;
            }
        }

        /// <summary>
        /// Clears the value of this <see cref="Lazy{T}"/> instance
        /// </summary>
        public void ClearValue() {
            this.value = default(T);
            this.isValueCreated = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Lazy{T}"/> class.
        /// </summary>
        /// <param name="valueFactory">The value factory.</param>
        public Lazy(Func<T> valueFactory) {
            this.valueFactory = valueFactory;
        }
    }
}
// ReSharper restore CheckNamespace