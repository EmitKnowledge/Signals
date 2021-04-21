using System;

namespace Signals.Aspects.DI
{
    /// <summary>
    /// Dependency resolver builder
    /// </summary>
    public interface IRegistrationService
    {
        /// <summary>
        /// Create service container by building registration service
        /// </summary>
        /// <returns></returns>
        IServiceContainer Build();

        /// <summary>
        /// Register interface with implementation
        /// </summary>
        /// <typeparam name="TDefinition"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        void Register<TDefinition, TImplementation>()
            where TDefinition : class
            where TImplementation : class, TDefinition;

        /// <summary>
        /// Register interface with implementation
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="implementationType"></param>
        void Register(Type serviceType, Type implementationType);

        /// <summary>
        /// Register type without interface
        /// </summary>
        /// <typeparam name="TImplementation"></typeparam>
        void Register<TImplementation>()
            where TImplementation : class;

        /// <summary>
        /// Register type without interface
        /// </summary>
        /// <param name="implementationType"></param>
        void Register(Type implementationType);

        /// <summary>
        /// Register interface with implementation as singleton
        /// </summary>
        /// <typeparam name="TDefinition"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        void RegisterSingleton<TDefinition, TImplementation>()
            where TDefinition : class
            where TImplementation : class, TDefinition;

        /// <summary>
        /// Register interface with implementation as singleton
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="implementationType"></param>
        void RegisterSingleton(Type serviceType, Type implementationType);

        /// <summary>
        /// Register type without interface as singleton
        /// </summary>
        /// <typeparam name="TImplementation"></typeparam>
        void RegisterSingleton<TImplementation>()
            where TImplementation : class;

        /// <summary>
        /// Register type without interface as singleton
        /// </summary>
        /// <param name="implementationType"></param>
        void RegisterSingleton(Type implementationType);

        /// <summary>
        /// Register interface with implementation instance
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="instance"></param>
        void Register(Type serviceType, object instance);

        /// <summary>
        /// Register interface with implementation instance
        /// </summary>
        /// <typeparam name="TDefinition"></typeparam>
        /// <param name="instance"></param>
        void Register<TDefinition>(TDefinition instance)
            where TDefinition : class;

		/// <summary>
		/// Register interface with implementation callback
		/// </summary>
		/// <param name="serviceType"></param>
		/// <param name="callback"></param>
		void Register(Type serviceType, Func<object> callback);

		/// <summary>
		/// Register interface with implementation callback
		/// </summary>
		/// <typeparam name="TDefinition"></typeparam>
		/// <param name="callback"></param>
		void Register<TDefinition>(Func<TDefinition> callback)
            where TDefinition : class;
    }
}
