using Signals.Aspects.CommunicationChannels.Configurations;

namespace Signals.Aspects.CommunicationChannels.Msmq.Configurations
{
    /// <summary>
    /// Configuration for MSMQ
    /// </summary>
    public class MsmqChannelConfiguration : IChannelConfiguration
    {
        /// <summary>
        /// Channel root path
        /// </summary>
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
