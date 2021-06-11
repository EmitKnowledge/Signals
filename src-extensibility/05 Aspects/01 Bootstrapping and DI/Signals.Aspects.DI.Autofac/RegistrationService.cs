using Autofac;
using System;

namespace Signals.Aspects.DI.Autofac
{
    /// <summary>
    /// Dependency resolver builder
    /// </summary>
    public class RegistrationService : IRegistrationService
    {
        /// <summary>
        /// Autofac container builder
        /// </summary>
        public ContainerBuilder Builder { get; set; }

        /// <summary>
        /// Autofac service container
        /// </summary>
        public ServiceContainer ServiceContainer { get; set; }

        /// <summary>
        /// Property selection strategy
        /// </summary>
        private PropertySelector propertySelector;

        /// <summary>
        /// CTOR
        /// </summary>
        public RegistrationService()
        {
            Builder = new ContainerBuilder();
            propertySelector = new PropertySelector();
        }

        /// <summary>
        /// Create service container by building registration service
        /// </summary>
        /// <returns></returns>
        public IServiceContainer Build()
        {
            if (ServiceContainer == null)
            {
                var container = Builder.Build();
                ServiceContainer = new ServiceContainer(container);
            }

            return ServiceContainer;
        }

        /// <summary>
        /// Register interface with implementation
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="implementationType"></param>
        public void Register(Type serviceType, Type implementationType)
        {
            Builder
                .RegisterType(implementationType)
                .As(serviceType)
                .PropertiesAutowired(propertySelector, true);
        }

        /// <summary>
        /// Register type without interface
        /// </summary>
        /// <typeparam name="TImplementation"></typeparam>
        public void Register<TImplementation>() where TImplementation : class
        {
            Builder
                .RegisterType<TImplementation>()
                .PropertiesAutowired(propertySelector, true);
        }

        /// <summary>
        /// Register type without interface
        /// </summary>
        /// <param name="implementationType"></param>
        public void Register(Type implementationType)
        {
            Builder
                .RegisterType(implementationType)
                .PropertiesAutowired(propertySelector, true);
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
            Builder
                .RegisterType<TImplementation>()
                .As<TDefinition>()
                .PropertiesAutowired(propertySelector, true);
        }

        /// <summary>
        /// Register interface with implementation as singleton
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="implementationType"></param>
        public void RegisterSingleton(Type serviceType, Type implementationType)
        {
            Builder
                .RegisterType(implementationType)
                .As(serviceType)
                .SingleInstance()
                .PropertiesAutowired(propertySelector, true);
        }

        /// <summary>
        /// Register type without interface as singleton
        /// </summary>
        /// <typeparam name="TImplementation"></typeparam>
        public void RegisterSingleton<TImplementation>() where TImplementation : class
        {
            Builder
                .RegisterType<TImplementation>()
                .SingleInstance()
                .PropertiesAutowired(propertySelector, true);
        }

        /// <summary>
        /// Register type without interface as singleton
        /// </summary>
        /// <param name="implementationType"></param>
        public void RegisterSingleton(Type implementationType)
        {
            Builder
                .RegisterType(implementationType)
                .SingleInstance()
                .PropertiesAutowired(propertySelector, true);
        }

        /// <summary>
        /// Register interface with implementation as singleton
        /// </summary>
        /// <typeparam name="TDefinition"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        public void RegisterSingleton<TDefinition, TImplementation>()
            where TDefinition : class
            where TImplementation : class, TDefinition
        {
            Builder
                .RegisterType<TImplementation>()
                .As<TDefinition>()
                .SingleInstance()
                .PropertiesAutowired(propertySelector, true);
        }

        /// <summary>
        /// Register interface with implementation instance
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="instance"></param>
        public void Register(Type serviceType, object instance)
        {
            Builder
                .RegisterInstance(instance)
                .As(serviceType)
                .PropertiesAutowired(propertySelector, true);
        }

        /// <summary>
        /// Register interface with implementation instance
        /// </summary>
        /// <typeparam name="TDefinition"></typeparam>
        /// <param name="instance"></param>
        public void Register<TDefinition>(TDefinition instance) where TDefinition : class
        {
            Builder
                .RegisterInstance(instance)
                .As<TDefinition>()
                .PropertiesAutowired(propertySelector, true);
        }

		/// <summary>
		/// Register interface with implementation callback
		/// </summary>
		/// <param name="serviceType"></param>
		/// <param name="callback"></param>
		public void Register(Type serviceType, Func<object> callback)
        {
            Builder
                .Register(ctx => callback())
                .As(serviceType)
                .PropertiesAutowired(propertySelector, true);
        }

		/// <summary>
		/// Register interface with implementation callback
		/// </summary>
		/// <typeparam name="TDefinition"></typeparam>
		/// <param name="callback"></param>
		public void Register<TDefinition>(Func<TDefinition> callback) where TDefinition : class
        {
            Builder
                .Register<TDefinition>(ctx => callback())
                .PropertiesAutowired(propertySelector, true);
        }
    }
}
