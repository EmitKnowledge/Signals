using System;
using System.Runtime.Serialization;

namespace Signals.Aspects.CommunicationChannels.MsSql
{
    /// <summary>
    /// Represents a system message in a queue
    /// </summary>
    [DataContract, Serializable]
    public class SystemMessage
    {
        /// <summary>
        /// Represents the message unique Id
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// Represents the date the message has been published
        /// </summary>
        [DataMember]
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Represents the name of the message queue
        /// </summary>
        [DataMember]
        public string MessageQueue { get; set; }

        /// <summary>
        /// Represents serialized message payload
        /// </summary>
        [DataMember]
        public string MessagePayload { get; set; }

        /// <summary>
        /// Represents the status of the message
        /// </summary>
        [DataMember]
        public SystemMessageStatus MessageStatus { get; set; }
    }
}
