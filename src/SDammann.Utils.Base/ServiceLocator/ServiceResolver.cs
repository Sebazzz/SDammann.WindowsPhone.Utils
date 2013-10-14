namespace SDammann.Utils.ServiceLocator {
    using System;
    using System.Collections.Generic;
    using System.Linq;


    /// <summary>
    ///   Helper class for resolving instances using a DI container
    /// </summary>
    public static class ServiceResolver {
        /// <summary>
        /// Contains mappings from interfaces/abstract types to concrete types
        /// </summary>
        private static readonly Dictionary<Type, Func<object>> Mappings = CreateServiceMappings();
        private static IServiceProviderEx _ServiceProvider = null;

        /// <summary>
        /// Sets the service resolver used to initialize view models and their dependencies
        /// </summary>
        /// <param name="serviceResolver">The service resolver. It may be null.</param>
        public static void SetServiceResolver(IServiceProviderEx serviceResolver) {
            _ServiceProvider = serviceResolver;
        }

        /// <summary>
        ///   Gets the service object of the specified type. If the service can not be resolved, <c>null</c> is returned.
        /// </summary>
        /// <typeparam name="TService"> The type of the service to resolve. </typeparam>
        /// <returns> An object of type <typeparamref name="TService" /> or <c>null</c> if the could not be resolved. </returns>
        public static TService GetService<TService>() where TService : class {
            if (_ServiceProvider == null) {
                return GetServiceInternal<TService>();
            }

            return _ServiceProvider.GetService<TService>() ?? GetServiceInternal<TService>();
        }

        /// <summary>
        /// Injects dependencies in a existing instance
        /// </summary>
        /// <param name="instance"></param>
        public static void InjectInstance(object instance) {
            if (_ServiceProvider != null) {
                _ServiceProvider.Inject(instance);
            }
        }

        /// <summary>
        /// Adds a mapping for the specified service. This is only to be used by libraries, and not phone apps themselves.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="factoryMethod">The factory method.</param>
        public static void AddMapping<TService>(Func<TService> factoryMethod) {
            Mappings [typeof (TService)] = () => factoryMethod.Invoke();
        }

        private static TService GetServiceInternal<TService>() where TService : class {
            // select a type first
            Type serviceType = typeof (TService);
            var q = from mapping in Mappings
                    let type = mapping.Key
                    let factoryMethod = mapping.Value
                    where serviceType.IsAssignableFrom(type)
                    select factoryMethod;

            Func<object> method = q.SingleOrDefault();

            return method != null ? (TService) method.Invoke() : null;
        }

        private static Dictionary<Type, Func<object>> CreateServiceMappings() {
            Dictionary<Type, Func<object>> mappings = new Dictionary<Type, Func<object>>();

            return mappings;
        }
    }
}