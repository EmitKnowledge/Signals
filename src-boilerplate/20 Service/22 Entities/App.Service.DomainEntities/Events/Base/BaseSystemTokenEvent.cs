using NodaTime;
using System;
using System.Runtime.Serialization;

namespace App.Service.DomainEntities.Events.Base
{
    /// <summary>
    /// Represent a token event class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    [DataContract]
    public class BaseSystemTokenEvent : BaseSystemEvent
    {
        /// <summary>
        /// Event validity date
        /// </summary>
        [DataMember]
        public Instant ValidUntil { get; set; }

        /// <summary>
        /// Tokien for validating user identity
        /// </summary>
        [DataMember]
        public string Token { get; set; }

        /// <summary>
        /// Id of the user to whom this event is designated
        /// </summary>
        [DataMember]
        public int UserId { get; set; }
    }
}