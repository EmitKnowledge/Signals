using Signals.Aspects.DI;
using Signals.Aspects.DI.Attributes;
using Signals.Aspects.DI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Signals.Aspects.Bootstrap
{
    public static class SystemBootstrapper
    {
        /// <summary>
        /// DI container that holds all service definiitons
        /// </summary>
        private static IServiceContainer _serviceContainer;

        /// <summary>
        /// Inject all public properties annotated with <see cref="ImportAttribute"/> 
        /// </summary>
        /// <param name="obj"></param>
        public static void Bootstrap(object obj)
        {
            if (obj == null) throw new NullReferenceException();

            PropertyInjector.Inject(_serviceContainer, obj);
        }

        /// <summary>
        /// Get instance of type
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        public static TService GetInstance<TService>() where TService : class
        {
            return _serviceContainer.GetInstance<TService>();
        }

        /// <summary>
        /// Get instance of type
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public static object GetInstance(Type serviceType)
        {
            return _serviceContainer.GetInstance(serviceType);
        }

	    /// <summary>
	    /// Initialize with container registrator that results in service container
	    /// </summary>
	    /// <param name="registrationService"></param>
	    /// <param name="rootAssembly"></param>
	    public static IServiceContainer Init(IRegistrationService registrationService, Assembly rootAssembly = null)
        {
            if (rootAssembly != null)
                AutoRegister(registrationService, rootAssembly);

            _serviceContainer = registrationService.Build();
            return _serviceContainer;
        }
        
        /// <summary>
        /// Auto register all exported types
        /// </summary>
        /// <param name="registrationService"></param>
        /// <param name="rootAssembly"></param>
        private static void AutoRegister(IRegistrationService registrationService, Assembly rootAssembly)
        {
            // get all types from all assemblies
            var types = rootAssembly.GetExportedTypes().ToList();
            types.AddRange(rootAssembly.GetReferencedAssemblies()
                               .Select(Assembly.Load)
                               .SelectMany(assembly => assembly.GetExportedTypes())
                               .OrderBy(x => x.FullName));

            // get all types with exported attribute
            var exportedTypePairs = types
                .Select(x => new KeyValuePair<Type, ExportAttribute>(x, x.GetCustomAttribute<ExportAttribute>()))
                .Where(x => x.Value != null)
                .ToList();

            // get distinct exports
            var distinctExports = exportedTypePairs.Select(x => x.Value?.DefinitionType.FullName).Distinct().ToList();

            if (exportedTypePairs.Count != distinctExports.Count)
            {
                // all groups that have more than 1 type with same implemented interface type
                var groups = exportedTypePairs.GroupBy(x => x.Value).Where(x => x.Count() > 1);
                foreach (var group in groups)
                {
                    throw new Exception($"{group.Key.DefinitionType.FullName} has multiple implementations: ");
                }
            }
            else
            {
                // autoregister all types
                foreach (var pair in exportedTypePairs)
                {
                    registrationService.Register(pair.Value.DefinitionType, pair.Key);
                }
            }
        }
    }
}
