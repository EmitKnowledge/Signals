using Signals.Aspects.CommunicationChannels.Configurations;

namespace Signals.Aspects.CommunicationChannels.Msmq.Configurations
{
    public class MsmqChannelConfiguration : IChannelConfiguration
    {
        public string ChannelPath { get; set; }

		/// <summary>
		/// CTOR
		/// </summary>
        public MsmqChannelConfiguration()
        {
            ChannelPath = @".\private$\";
        }
    }
}
