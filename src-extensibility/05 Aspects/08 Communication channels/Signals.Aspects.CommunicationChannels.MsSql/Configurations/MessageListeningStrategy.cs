using System;
using System.Runtime.Serialization;

namespace Signals.Aspects.CommunicationChannels.MsSql.Configurations
{
    [DataContract, Serializable]
    public enum MessageListeningStrategy
    {
        /// <summary>
        /// Represents strategy where the client doesn't listen for events
        /// </summary>
        [EnumMember]
        None,

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
