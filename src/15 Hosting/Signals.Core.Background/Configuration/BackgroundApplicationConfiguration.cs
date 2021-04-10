using Signals.Aspects.Configuration;
using Signals.Core.Background.Configuration.ConfigurationSegments;
using System.ComponentModel.DataAnnotations;

namespace Signals.Core.Background.Configuration
{
    /// <summary>
    /// Background application information
    /// </summary>
    public class BackgroundApplicationConfiguration : BaseConfiguration<BackgroundApplicationConfiguration>
    {
        /// <summary>
        /// Configuration key
        /// </summary>
        public override string Key => nameof(BackgroundApplicationConfiguration);

        /// <summary>
        /// Notification configuration on background startup
        /// </summary>
        public StartupNotificationConfiguration StartupNotificationConfiguration { get; set; }
    }
}
