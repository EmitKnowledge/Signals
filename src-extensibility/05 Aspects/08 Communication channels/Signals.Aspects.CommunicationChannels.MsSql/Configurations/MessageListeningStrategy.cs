using System;
using System.Runtime.Serialization;

namespace Signals.Aspects.CommunicationChannels.MsSql.Configurations
{
    [DataContract, Serializable]
    public enum MessageListeningStrategy
    {
	    /// <summary>
        /// Represents long polling listening strategy
        /// </summary>
        [EnumMember]
        LongPolling,

        /// <summary>
        /// Represents listening strategy with sql broker
        /// </summary>
        [EnumMember]
        Broker
    }
}
