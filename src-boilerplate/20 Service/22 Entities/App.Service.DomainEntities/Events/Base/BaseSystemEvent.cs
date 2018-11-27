using System;
using System.Runtime.Serialization;
using App.Service.DomainEntities.Base;

namespace App.Service.DomainEntities.Events.Base
{
    /// <summary>
    /// Represent system event class
    /// </summary>
    [Serializable]
    [DataContract]
    public class BaseSystemEvent : BaseDomainEntity
    {
        private string _mEventName;

        /// <summary>
        /// Name of the event
        /// </summary>
        [DataMember]
        public string EventName
        {
            get
            {
                if (string.IsNullOrEmpty(_mEventName))
                {
                    return this.GetType().Name;
                }

                return _mEventName;
            }
            set
            {
                _mEventName = value;
            }
        }

        /// <summary>
        /// Event sender
        /// </summary>
        [DataMember]
        public string Sender { get; set; }

        /// <summary>
        /// Event payload data
        /// </summary>
        [DataMember]
        public string PayloadData { get; set; }

        /// <summary>
        /// Indicate that the event has been processed by its processor
        /// </summary>
        [DataMember]
        public bool IsProcessed { get; set; }
    }
}
