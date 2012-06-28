namespace SDammann.Utils {
    using System;
    using System.Diagnostics;
    using System.Threading;
    using Threading;


    /// <summary>
    /// Base class for asynchronous operations results
    /// </summary>
    public class AsyncResultBase : IAsyncResult {
        private readonly object userToken;
        private readonly ManualResetEvent waitHandle;
        private readonly AsyncCallback callback;
        
        private bool isCompleted;
        private Exception errorException;

        /// <summary>
        /// Gets the error exception that may have occurred
        /// </summary>
        public Exception ErrorException {
            [DebuggerStepThrough]
            get { return this.errorException; }
        }

        /// <summary>
        /// Gets a value indicating whether the asynchronous operation completed without error
        /// </summary>
        public bool HasError {
            [DebuggerStepThrough]
            get { return this.errorException != null; }
        }

        /// <summary>
        /// Gets a value that indicates whether the asynchronous operation has completed.
        /// </summary>
        /// <returns>true if the operation is complete; otherwise, false.</returns>
        public bool IsCompleted {
            [DebuggerStepThrough]
            get { return this.isCompleted; }
        }

        /// <summary>
        /// Gets a <see cref="T:System.Threading.WaitHandle"/> that is used to wait for an asynchronous operation to complete.
        /// </summary>
        /// <returns>A wait handle that is used to wait for an asynchronous operation to complete.</returns>
        public WaitHandle AsyncWaitHandle {
            [DebuggerStepThrough]
            get { return this.waitHandle; }
        }

        /// <summary>
        /// Gets a user-defined object that qualifies or contains information about an asynchronous operation.
        /// </summary>
        /// <returns>A user-defined object that qualifies or contains information about an asynchronous operation.</returns>
        public object AsyncState {
            [DebuggerStepThrough]
            get { return this.userToken; }
        }

        /// <summary>
        /// Gets a value that indicates whether the asynchronous operation completed synchronously.
        /// </summary>
        /// <returns>true if the asynchronous operation completed synchronously; otherwise, false.</returns>
        public bool CompletedSynchronously {
            [DebuggerStepThrough]
            get { return false; }
        }

        /// <summary>
        /// Signals that the asynchronous operation is completed, and invokes the callback and sets the <see cref="IAsyncResult.AsyncWaitHandle"/>
        /// </summary>
        protected void SetCompleted() {
            if (this.isCompleted) {
                throw new InvalidOperationException("Operation is already completed");
            }

            this.isCompleted = true;
            this.waitHandle.Set();
            
            if (this.callback != null) {
                this.callback.InvokeSafe(this);
            }
        }

        /// <summary>
        /// Signals that the asynchronous operation is completed, and invokes the callback and sets the <see cref="IAsyncResult.AsyncWaitHandle"/>
        /// </summary>
        /// <param name="error"></param>
        public void SetCompleted(Exception error) {
            this.errorException = error;

            this.SetCompleted();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncResultBase"/> class.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="userToken">The user token.</param>
        [DebuggerStepThrough]
        public AsyncResultBase(AsyncCallback callback, object userToken) {
            this.callback = callback;
            this.userToken = userToken;

            this.waitHandle = new ManualResetEvent(false);
        }
    }
}
