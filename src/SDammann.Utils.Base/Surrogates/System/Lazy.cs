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
        private readonly Func<T> _valueFactory; 
        private bool _isValueCreated;
        private T _value;

        /// <summary>
        /// Gets a value that indicates whether a value has been created for this <see cref="Lazy{T}"/> instance.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if a value created; otherwise, <c>false</c>.
        /// </value>
        public bool IsValueCreated {
            [DebuggerStepThrough]
            get { return this._isValueCreated; }
        }

        /// <summary>
        /// Gets the lazily initialized value of the current <see cref="Lazy{T}"/> instance.
        /// </summary>
        public T Value {
            [DebuggerStepThrough]
            get {
                if (!this._isValueCreated) {
                    this._value = this._valueFactory.Invoke();
                    this._isValueCreated = true;
                }

                return this._value;
            }
        }

        /// <summary>
        /// Clears the value of this <see cref="Lazy{T}"/> instance
        /// </summary>
        public void ClearValue() {
            this._value = default(T);
            this._isValueCreated = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Lazy{T}"/> class.
        /// </summary>
        /// <param name="valueFactory">The value factory.</param>
        public Lazy(Func<T> valueFactory) {
            this._valueFactory = valueFactory;
        }
    }
}
// ReSharper restore CheckNamespace