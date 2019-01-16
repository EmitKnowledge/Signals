using System;
using System.Runtime.Serialization;

namespace Signals.Aspects.CommunicationChannels.MsSql
{
    /// <summary>
    /// Represents a system message status
    /// </summary>
    [DataContract, Serializable]
    public enum SystemMessageStatus
    {
        /// <summary>
        /// Represents pending message status
        /// </summary>
        [EnumMember]
        Pending,

        /// <summary>
        /// Represents processing message status
        /// </summary>
        [EnumMember]
        Processing,

        /// <summary>
        /// Represents processed message status
        /// </summary>
        [EnumMember]
        Processed
    }
}
