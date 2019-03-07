using Signals.Aspects.CommunicationChannels.Configurations;

namespace Signals.Aspects.CommunicationChannels.ServiceBus.Configurations
{
    /// <summary>
    /// Configuration for service bus
    /// </summary>
    public class ServiceBusChannelConfiguration : IChannelConfiguration
    {
        /// <summary>
        /// CTOR
        /// </summary>
        public ServiceBusChannelConfiguration()
        {
            ChannelPrefix = "";
        }

        /// <summary>
        /// Service bus connection string
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Service bus channel prefix
        /// </summary>
        public string ChannelPrefix { get; set; }
    }
}
