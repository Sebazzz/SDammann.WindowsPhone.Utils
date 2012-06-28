namespace SDammann.Utils {
    using System;
    using System.Diagnostics;


    /// <summary>
    /// Class for asynchronous operation results that return no value
    /// </summary>
    public sealed class VoidAsyncResult : AsyncResultBase {
        /// <summary>
        /// Signals that the asynchronous operation is completed, and invokes the callback and sets the <see cref="IAsyncResult.AsyncWaitHandle"/>
        /// </summary>
        public new void SetCompleted() {
            base.SetCompleted();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VoidAsyncResult"/> class.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="userToken">The user token.</param>
        [DebuggerStepThrough]
        public VoidAsyncResult(AsyncCallback callback, object userToken)
                : base(callback, userToken) {
        }
    }
}