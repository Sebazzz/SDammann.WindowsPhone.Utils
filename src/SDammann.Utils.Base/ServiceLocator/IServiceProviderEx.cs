namespace SDammann.Utils.ServiceLocator {
    using System;


    /// <summary>
    ///   Defines a mechanism for retrieving a service object; that is, an object that provides custom support to other objects.
    /// </summary>
    public interface IServiceProviderEx : IServiceProvider {
        /// <summary>
        ///   Injects the existing instance with dependencies
        /// </summary>
        /// <param name="instance"> The instance. </param>
        void Inject (object instance);
    }
}