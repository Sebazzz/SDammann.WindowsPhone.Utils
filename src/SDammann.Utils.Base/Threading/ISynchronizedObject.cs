namespace SDammann.Utils.Threading {
    using System.Threading;


    /// <summary>
    ///   Defines the synchronization context on which the object was created
    /// </summary>
    public interface ISynchronizedObject {
        /// <summary>
        ///   Gets the synchronization context on which this object was created
        /// </summary>
        SynchronizationContext ObjectSynchronizationContext { get; }
    }
}