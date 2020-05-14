using Signals.Core.Common.Reflection;
using Signals.Features.Base.Configurations.Feature;
using Signals.Features.Base.Configurations.MicroService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Signals.Features.Base.Configurations.Registration
{
    /// <summary>
    /// 
    /// </summary>
    public class FluentFeatureRegistrationService
    {
        private readonly Action<Type, Func<object>> _registrationCallback;
        private readonly List<Type> _assemblyTypes;

        /// <summary>
        /// CTOR
        /// </summary>
        public FluentFeatureRegistrationService(Action<Type, Func<object>> registrationCallback)
        {
            _registrationCallback = registrationCallback;

            var requiringType = typeof(IFeature);
            _assemblyTypes = Directory
                .GetFiles(AppContext.BaseDirectory, "*.dll")
                .Select(Assembly.LoadFrom)
                .SelectMany(assembly => assembly.LoadAllTypesFromAssembly())
                .Where(x => (x.GetInterfaces().Contains(requiringType) || x.IsSubclassOf(requiringType)) && !x.IsInterface && !x.IsAbstract)
                .ToList();
        }

        /// <summary>
        /// Initialize feature configuration
        /// </summary>
        /// <param name="featureConfiguration"></param>
        /// <param name="fluentConfiguration"></param>
        /// <returns></returns>
        public FluentFeatureConfigurationForService UseFeature(IFeatureConfiguration featureConfiguration)
        {
            var featureType = featureConfiguration.GetType();

            var serviceType = _assemblyTypes
                .Where(x => x
                    .GetConstructors()
                    .Any(ctor => ctor
                            .GetParameters()
                            .Any() && 
                        ctor
                            .GetParameters()
                            .All(param => param.ParameterType == featureType)))
                .Distinct()
                .SingleOrDefault();

            var instance = Activator.CreateInstance(serviceType, featureConfiguration);

            foreach (var interfaceType in serviceType.GetInterfaces())
            {
                var proxy = FeatureClient.CreateProxy(instance, interfaceType, featureConfiguration);
                _registrationCallback(interfaceType, () => proxy);
            }

            return new FluentFeatureConfigurationForService(featureConfiguration, this);
        }

        public class FluentFeatureConfigurationForService
        {
            private readonly IFeatureConfiguration _featureConfiguration;
            private readonly FluentFeatureRegistrationService _fluentConfiguration;

            /// <summary>
            /// CTOR
            /// </summary>
            /// <param name="featureConfiguration"></param>
            public FluentFeatureConfigurationForService(IFeatureConfiguration featureConfiguration, FluentFeatureRegistrationService fluentConfiguration)
            {
                _featureConfiguration = featureConfiguration;
                _fluentConfiguration = fluentConfiguration;
            }

            /// <summary>
            /// Use feature as library
            /// </summary>
            public FluentFeatureRegistrationService AsLibrary()
            {
                return _fluentConfiguration;
            }

            /// <summary>
            /// Use feature as microservice
            /// </summary>
            public FluentFeatureRegistrationService AsMicroService(MicroServiceConfiguration microServiceConfiguration)
            {
                _featureConfiguration.MicroServiceConfiguration = microServiceConfiguration;
                return _fluentConfiguration;
            }
        }
    }
}
