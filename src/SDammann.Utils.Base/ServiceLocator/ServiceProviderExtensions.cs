namespace SDammann.Utils.ServiceLocator {
    using System;


    /// <summary>
    ///   Contains helper extensions for <see cref="IServiceProvider" />
    /// </summary>
    public static class ServiceProviderExtensions {
        /// <summary>
        ///   Gets the service object of the specified type. If the service can not be resolved, <c>null</c> is returned.
        /// </summary>
        /// <typeparam name="TService"> The type of the service to resolve. </typeparam>
        /// <param name="serviceProvider"> The service provider to use in order to resolve the service.. </param>
        /// <returns> An object of type <typeparamref name="TService" /> or <c>null</c> if the could not be resolved. </returns>
        public static TService GetService<TService> (this IServiceProvider serviceProvider) where TService : class {
            if (serviceProvider == null) {
                throw new ArgumentNullException("serviceProvider");
            }

            return serviceProvider.GetService(typeof (TService)) as TService;
        }
    }
}