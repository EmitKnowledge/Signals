using App.Service.DomainEntities.Users;
using System;
using System.Runtime.Serialization;

namespace App.Service.DomainEntities.Events.Users
{
    [Serializable]
    [DataContract]
    public class UserNotificationPayload<T>
    {
        /// <summary>
        /// Represent the user to whom this notification is created
        /// </summary>
        [DataMember]
        public User From { get; set; }

        /// <summary>
        /// Represent the user to whom this notification is created
        /// </summary>
        [DataMember]
        public User To { get; set; }

        /// <summary>
        /// Represent the entity which created the notification
        /// </summary>
        [DataMember]
        public T ConnectedEntity { get; set; }
    }
}