using System;
using System.Collections.Generic;
using System.Text;

namespace Signals.Features.Base.Configurations
{
    /// <summary>
    /// 
    /// </summary>
    public class FluentFeatureConfiguration
    {
        private readonly List<IFeatureConfiguration> _featureConfigurations;

        /// <summary>
        /// CTOR
        /// </summary>
        public FluentFeatureConfiguration()
        {
            _featureConfigurations = new List<IFeatureConfiguration>();
        }

        /// <summary>
        /// Initialize feature configuration
        /// </summary>
        /// <param name="featureConfiguration"></param>
        /// <returns></returns>
        public FluentFeatureConfigurationForService UseService(IFeatureConfiguration featureConfiguration)
        {
            _featureConfigurations.Add(featureConfiguration);
            return new FluentFeatureConfigurationForService(featureConfiguration, this);
        }

        /// <summary>
        /// Build feature configurations
        /// </summary>
        /// <returns></returns>
        public List<IFeatureConfiguration> Build()
        {
            return _featureConfigurations;
        }

        public class FluentFeatureConfigurationForService
        {
            private readonly IFeatureConfiguration _featureConfiguration;
            private readonly FluentFeatureConfiguration _fluentConfiguration;

            /// <summary>
            /// CTOR
            /// </summary>
            /// <param name="featureConfiguration"></param>
            public FluentFeatureConfigurationForService(IFeatureConfiguration featureConfiguration, FluentFeatureConfiguration fluentConfiguration)
            {
                _featureConfiguration = featureConfiguration;
                _fluentConfiguration = fluentConfiguration;
            }

            /// <summary>
            /// Use feature as library
            /// </summary>
            public FluentFeatureConfiguration AsLibrary()
            {
                return _fluentConfiguration;
            }

            /// <summary>
            /// Use feature as microservice
            /// </summary>
            public FluentFeatureConfiguration AsMicroService(MicroServiceConfiguration microServiceConfiguration)
            {
                _featureConfiguration.MicroServiceConfiguration = microServiceConfiguration;
                return _fluentConfiguration;
            }
        }
    }
}
