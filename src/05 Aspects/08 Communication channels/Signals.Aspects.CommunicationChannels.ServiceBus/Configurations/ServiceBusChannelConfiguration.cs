using Signals.Aspects.CommunicationChannels.Configurations;

namespace Signals.Aspects.CommunicationChannels.ServiceBus.Configurations
{
    public class ServiceBusChannelConfiguration : IChannelConfiguration
    {
        public string ConnectionString { get; set; }
    }
}
