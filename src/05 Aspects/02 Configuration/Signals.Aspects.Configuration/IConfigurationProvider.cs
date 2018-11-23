﻿namespace Signals.Aspects.Configuration
{
    public interface IConfigurationProvider
    {
        /// <summary>
        /// Indicates if the configuration should be reloaded on property access
        /// </summary>
        bool ReloadOnAccess { get; set; }

        /// <summary>
        /// Loads the configuration into memory
        /// </summary>
        BaseConfiguration<T> Load<T>(string key) where T : BaseConfiguration<T>, new();

        /// <summary>
        /// Reloads the configuration
        /// </summary>
        BaseConfiguration<T> Reload<T>(string key) where T : BaseConfiguration<T>, new();
    }
}