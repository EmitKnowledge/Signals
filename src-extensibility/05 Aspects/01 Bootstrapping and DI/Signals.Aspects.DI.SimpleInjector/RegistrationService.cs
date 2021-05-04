using SimpleInjector;
using SimpleInjector.Lifestyles;
using System;

namespace Signals.Aspects.DI.SimpleInjector
{
    /// <summary>
    /// Dependency resolver builder
    /// </summary>
    public class RegistrationService : IRegistrationService
    {
        private readonly bool _verifyOnBuild;

        /// <summary>
        /// SimpleInjector container builder
        /// </summary>
        public Container Builder { get; set; }

        /// <summary>
        /// SimpleInjector service container
        /// </summary>
        public ServiceContainer ServiceContainer { get; set; }

        /// <summary>
        /// CTOR
        /// <param name="verifyOnBuild"></param>
        /// </summary>
        public RegistrationService(bool verifyOnBuild = true)
        {
            Builder = new Container();
            Builder.Options.ConstructorResolutionBehavior = new MostResolvableConstructorBehavior(Builder);
            Builder.Options.AllowOverridingRegistrations = true;
            Builder.Options.DefaultLifestyle = Lifestyle.Transient;
            Builder.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            Builder.Options.PropertySelectionBehavior = new ImportPropertySelectionBehavior(Builder);
            this._verifyOnBuild = verifyOnBuild;
        }

        /// <summary>
        /// Create service container by building registration service
        /// </summary>
        /// <returns></returns>
        public IServiceContainer Build()
        {
	        if (ServiceContainer != null) return ServiceContainer;
            if (_verifyOnBuild) Builder.Verify();
	        ServiceContainer = new ServiceContainer(Builder);

	        return ServiceContainer;
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
            Builder.Register<TDefinition, TImplementation>();
        }

        /// <summary>
        /// Register interface with implementation
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="implementationType"></param>
        public void Register(Type serviceType, Type implementationType)
        {
            Builder.Register(serviceType, implementationType);
        }

        /// <summary>
        /// Register type without interface
        /// </summary>
        /// <typeparam name="TImplementation"></typeparam>
        public void Register<TImplementation>() where TImplementation : class
        {
            Builder.Register<TImplementation>();
        }

        /// <summary>
        /// Register type without interface
        /// </summary>
        /// <param name="implementationType"></param>
        public void Register(Type implementationType)
        {
            Builder.Register(implementationType);
        }

        /// <summary>
        /// Register interface with implementation
        /// </summary>
        /// <typeparam name="TDefinition"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        public void RegisterSingleton<TDefinition, TImplementation>()
            where TDefinition : class
            where TImplementation : class, TDefinition
        {
            Builder.RegisterSingleton<TDefinition, TImplementation>();
        }

        /// <summary>
        /// Register interface with implementation as singleton
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="implementationType"></param>
        public void RegisterSingleton(Type serviceType, Type implementationType)
        {
            Builder.RegisterSingleton(serviceType, implementationType);
        }

        /// <summary>
        /// Register type without interface as singleton
        /// </summary>
        /// <typeparam name="TImplementation"></typeparam>
        public void RegisterSingleton<TImplementation>() where TImplementation : class
        {
            Builder.RegisterSingleton<TImplementation>();
        }

        /// <summary>
        /// Register type without interface as singleton
        /// </summary>
        /// <param name="implementationType"></param>
        public void RegisterSingleton(Type implementationType)
        {
            Builder.RegisterSingleton(implementationType);
        }

        /// <summary>
        /// Register interface with implementation instance as singleton
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="instance"></param>
        public void Register(Type serviceType, object instance)
        {
            Builder.RegisterInstance(serviceType, instance);
        }

        /// <summary>
        /// Register interface with implementation instance
        /// </summary>
        /// <typeparam name="TDefinition"></typeparam>
        /// <param name="instance"></param>
        public void Register<TDefinition>(TDefinition instance) where TDefinition : class
        {
            Builder.RegisterInstance(instance);
        }

		/// <summary>
		/// Register interface with implementation callback
		/// </summary>
		/// <param name="serviceType"></param>
		/// <param name="callback"></param>
		public void Register(Type serviceType, Func<object> callback)
        {
            Builder.Register(serviceType, callback);
        }

		/// <summary>
		/// Register interface with implementation callback
		/// </summary>
		/// <typeparam name="TDefinition"></typeparam>
		/// <param name="callback"></param>
		public void Register<TDefinition>(Func<TDefinition> callback) where TDefinition : class
        {
            Builder.Register(callback);
        }
    }
}
