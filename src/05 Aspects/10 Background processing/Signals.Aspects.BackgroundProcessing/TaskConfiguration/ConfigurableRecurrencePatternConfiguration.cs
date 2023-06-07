using System;

namespace Signals.Aspects.BackgroundProcessing.TaskConfiguration
{
	/// <summary>
	/// Advanced configuration for recurracne porovider
	/// </summary>
	public sealed class ConfigurableRecurrencePatternConfiguration : RecurrencePatternConfiguration
	{
        private readonly Func<RecurrencePatternConfiguration> provider;

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="provider"></param>
        public ConfigurableRecurrencePatternConfiguration(Func<RecurrencePatternConfiguration> provider) : base()
		{
            this.provider = provider;
        }

        /// <summary>
        /// Get configuration
        /// </summary>
        /// <returns></returns>
        public override RecurrencePatternConfiguration GetInstance()
        {
            return provider();
        }
    }
}
