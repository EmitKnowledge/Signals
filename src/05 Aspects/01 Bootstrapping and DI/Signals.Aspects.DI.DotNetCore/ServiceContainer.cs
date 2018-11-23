using Signals.Aspects.DI.Helpers;
using System;

namespace Signals.Aspects.DI.DotNetCore
{
    /// <summary>
    /// Dependency resolver
    /// </summary>
    public class ServiceContainer : IServiceContainer
    {
        /// <summary>
        /// DotNet Core  container
        /// </summary>
        public IServiceProvider Container { get; }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="container"></param>
        public ServiceContainer(IServiceProvider container)
        {
            Container = container;
        }

        /// <summary>
        /// Get instance of type
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        public TService GetInstance<TService>() where TService : class
        {
            var instance = Container.GetService(typeof(TService)) as TService;
            PropertyInjector.Inject(this, instance);
            return instance;
        }

        /// <summary>
        /// Get instance of type
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public object GetInstance(Type serviceType)
        {
            var instance = Container.GetService(serviceType);
            PropertyInjector.Inject(this, instance);
            return instance;
        }
    }
}
