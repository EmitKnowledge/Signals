using Autofac;
using System;

namespace Signals.Aspects.DI.Autofac
{
    /// <summary>
    /// Dependency resolver
    /// </summary>
    public class ServiceContainer : IServiceContainer
    {
        /// <summary>
        /// Autofac container
        /// </summary>
        public IContainer Container { get; }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="container"></param>
        public ServiceContainer(IContainer container)
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
            if (!Container.IsRegistered<TService>()) return null;
            return Container.Resolve<TService>();
        }

        /// <summary>
        /// Get instance of type
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public object GetInstance(Type serviceType)
        {
            if (!Container.IsRegistered(serviceType)) return null;
            return Container.Resolve(serviceType);
        }
    }
}
