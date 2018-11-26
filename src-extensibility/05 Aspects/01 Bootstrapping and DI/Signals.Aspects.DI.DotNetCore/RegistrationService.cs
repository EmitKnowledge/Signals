using Microsoft.Extensions.DependencyInjection;
using System;

namespace Signals.Aspects.DI.DotNetCore
{
    /// <summary>
    /// Dependency resolver builder
    /// </summary>
    public class RegistrationService : IRegistrationService
    {
        /// <summary>
        /// DotNet Core container builder
        /// </summary>
        public IServiceCollection Builder { get; set; }

        /// <summary>
        /// DotNet Core service container
        /// </summary>
        public ServiceContainer ServiceContainer { get; set; }

        /// <summary>
        /// CTOR
        /// </summary>
        public RegistrationService(IServiceCollection existingCollection = null)
        {
            Builder = existingCollection ?? new ServiceCollection();
        }

        /// <summary>
        /// Create service container by building registration service
        /// </summary>
        /// <returns></returns>
        public IServiceContainer Build()
        {
	        if (ServiceContainer != null) return ServiceContainer;
	        IServiceProvider provider = Builder.BuildServiceProvider();
	        ServiceContainer = new ServiceContainer(provider);

	        return ServiceContainer;
        }

        /// <summary>
        /// Register interface with implementation
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="implementationType"></param>
        public void Register(Type serviceType, Type implementationType)
        {
            Builder.AddTransient(serviceType, implementationType);
        }

        /// <summary>
        /// Register type without interface
        /// </summary>
        /// <typeparam name="TImplementation"></typeparam>
        public void Register<TImplementation>() where TImplementation : class
        {
            Builder.AddTransient<TImplementation>();
        }

        /// <summary>
        /// Register type without interface
        /// </summary>
        /// <param name="implementationType"></param>
        public void Register(Type implementationType)
        {
            Builder.AddTransient(implementationType);
        }

        /// <summary>
        /// Register interface with implementation
        /// </summary>
        /// <typeparam name="TDefinition"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        public void Register<TDefinition, TImplementation>()
            where TDefinition : class
            where TImplementation : class, TDefinition
        {
            Builder.AddTransient<TDefinition, TImplementation>();
        }

        /// <summary>
        /// Register interface with implementation instance
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="instance"></param>
        public void Register(Type serviceType, object instance)
        {
            Builder.AddTransient(serviceType, provider => instance);
        }

        /// <summary>
        /// Register interface with implementation instance
        /// </summary>
        /// <typeparam name="TDefinition"></typeparam>
        /// <param name="instance"></param>
        public void Register<TDefinition>(TDefinition instance) where TDefinition : class
        {
            Builder.AddTransient(provider => instance);
        }

		/// <summary>
		/// Register interface with implementation callback
		/// </summary>
		/// <param name="serviceType"></param>
		/// <param name="callback"></param>
		public void Register(Type serviceType, Func<object> callback)
        {
            Builder.AddTransient(serviceType, provider => callback());
        }

		/// <summary>
		/// Register interface with implementation callback
		/// </summary>
		/// <typeparam name="TDefinition"></typeparam>
		/// <param name="callback"></param>
		public void Register<TDefinition>(Func<TDefinition> callback) where TDefinition : class
        {
            Builder.AddTransient(provider => callback());
        }
    }
}
