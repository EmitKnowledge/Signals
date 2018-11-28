using Signals.Aspects.CommunicationChannels.Configurations;

namespace Signals.Aspects.CommunicationChannels.ServiceBus.Configurations
{
    /// <summary>
    /// Configuration for service bus
    /// </summary>
    public class ServiceBusChannelConfiguration : IChannelConfiguration
    {
        /// <summary>
        /// Service bus connection string
        /// </summary>
        public string ConnectionString { get; set; }
    }
}
