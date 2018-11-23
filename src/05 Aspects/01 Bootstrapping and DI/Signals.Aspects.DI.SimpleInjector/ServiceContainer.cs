using SimpleInjector;
using System;

namespace Signals.Aspects.DI.SimpleInjector
{
    /// <summary>
    /// Dependency resolver
    /// </summary>
    public class ServiceContainer : IServiceContainer
    {
        /// <summary>
        /// SimpleInjector  container
        /// </summary>
        public Container Container { get; }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="container"></param>
	    public ServiceContainer(Container container)
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
            if (Container.GetRegistration(typeof(TService)) == null) return null;
            return Container.GetInstance<TService>();
        }

	    /// <summary>
	    /// Get instance of type
	    /// </summary>
	    /// <param name="serviceType"></param>
	    /// <returns></returns>
	    public object GetInstance(Type serviceType)
        {
            if (Container.GetRegistration(serviceType) == null) return null;
            return Container.GetInstance(serviceType);
        }
    }
}
