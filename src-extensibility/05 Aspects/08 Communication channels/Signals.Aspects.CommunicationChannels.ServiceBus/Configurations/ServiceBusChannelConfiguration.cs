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
            MaxConcurrentCalls = 1;
        }

        /// <summary>
        /// Service bus connection string
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Service bus channel prefix
        /// </summary>
        public string ChannelPrefix { get; set; }

        /// <summary>
        /// Number of threads to receive and execute messages in parallel manner
        /// </summary>
        public int MaxConcurrentCalls { get; set; }
    }
}
