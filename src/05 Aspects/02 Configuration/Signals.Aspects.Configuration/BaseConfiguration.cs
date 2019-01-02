using System;

namespace Signals.Aspects.Configuration
{
    /// <summary>
    /// Base configuration element
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseConfiguration<T> where T : BaseConfiguration<T>, new()
    {
        private static T _instance;

        /// <summary>
        /// Provided instance
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_instance == null) throw new Exception("Please set a configuraiton provider before accessing the instnace");
                if (_instance.Provider.ReloadOnAccess)
                {
                    var provider = _instance.Provider;
                    _instance = _instance.Provider.Reload<T>(_instance.Key) as T;
                    _instance.Provider = provider;
                }

                return _instance;
            }
        }

        /// <summary>
        /// The configuration's name
        /// </summary>
        public abstract string Key { get; }

        /// <summary>
        /// Active provider
        /// </summary>
        private IConfigurationProvider Provider { get; set; }

        /// <summary>
        /// Sets up configuration provider for loading configuration
        /// </summary>
        /// <param name="provider"></param>
        public static void UseProvider(IConfigurationProvider provider)
        {
            _instance = (T)provider.Load<T>(new T().Key);
            _instance.Provider = provider;
        }

        /// <summary>
        /// Updates configuration
        /// </summary>
        public static void Update(T newInstanceValue)
        {
            if (newInstanceValue == null) throw new ArgumentNullException(nameof(newInstanceValue));

            newInstanceValue.Provider = _instance.Provider;
            _instance = newInstanceValue;

            _instance.Provider.Update(_instance);
        }
    }
}