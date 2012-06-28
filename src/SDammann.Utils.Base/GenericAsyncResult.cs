namespace SDammann.Utils {
    using System;
    using System.Diagnostics;


    /// <summary>
    ///   Represents the result of the asynchronous callback
    /// </summary>
    /// <typeparam name="TResult"> </typeparam>
    public sealed class GenericAsyncResult<TResult> : AsyncResultBase where TResult : class {
        private TResult resultObject;

        /// <summary>
        ///   Gets the result.
        /// </summary>
        public TResult Result {
            [DebuggerStepThrough]
            get { return this.resultObject; }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="GenericAsyncResult{TResult}" /> class.
        /// </summary>
        /// <param name="callback"> The callback. </param>
        /// <param name="userToken"> The user token. </param>
        public GenericAsyncResult (AsyncCallback callback, object userToken)
                : base(callback, userToken) {
        }

        /// <summary>
        ///   Signals that the asynchronous operation is completed, and invokes the callback and sets the <see
        ///    cref="IAsyncResult.AsyncWaitHandle" />
        /// </summary>
        /// <param name="result"> The result. </param>
        public void SetCompleted (TResult result) {
            this.resultObject = result;

            this.SetCompleted();
        }
    }
}