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
    /// Fluent feature registrator
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
        /// <returns></returns>
        public FluentFeatureConfigurationForService UseFeature(BaseFeatureConfiguration featureConfiguration)
        {
            return new FluentFeatureConfigurationForService(featureConfiguration, this);
        }

        /// <summary>
        /// Fluent configuration builder
        /// </summary>
        public class FluentFeatureConfigurationForService
        {
            private readonly BaseFeatureConfiguration _featureConfiguration;
            private readonly FluentFeatureRegistrationService _fluentConfiguration;

            /// <summary>
            /// CTOR
            /// </summary>
            /// <param name="featureConfiguration"></param>
            /// <param name="fluentConfiguration"></param>
            public FluentFeatureConfigurationForService(BaseFeatureConfiguration featureConfiguration, FluentFeatureRegistrationService fluentConfiguration)
            {
                _featureConfiguration = featureConfiguration;
                _fluentConfiguration = fluentConfiguration;
            }

            /// <summary>
            /// Use feature as library
            /// </summary>
            public FluentFeatureRegistrationService AsLibrary()
            {
                _featureConfiguration.MicroServiceConfiguration = null;

                Register(createProxy: false);

                return _fluentConfiguration;
            }

            /// <summary>
            /// Use feature as microservice
            /// </summary>
            public FluentFeatureRegistrationService AsMicroService(MicroServiceConfiguration microServiceConfiguration)
            {
                _featureConfiguration.MicroServiceConfiguration = microServiceConfiguration;

                Register(createProxy: true);

                return _fluentConfiguration;
            }

            /// <summary>
            /// Register the feature
            /// </summary>
            /// <param name="createProxy"></param>
            private void Register(bool createProxy)
            {
                var featureType = _featureConfiguration.GetType();

                var serviceType = _fluentConfiguration._assemblyTypes
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

                var interfaceType = serviceType.GetInterfaces().FirstOrDefault();

                object service = Activator.CreateInstance(serviceType, _featureConfiguration);

                if (createProxy)
                    service = FeatureClient.CreateProxy(service, interfaceType, _featureConfiguration);

                _fluentConfiguration._registrationCallback(interfaceType, () => service);
            }
        }
    }
}
