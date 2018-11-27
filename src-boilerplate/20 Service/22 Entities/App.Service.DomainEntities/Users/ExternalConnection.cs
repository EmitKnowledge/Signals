using App.Service.DomainEntities.Base;
using System;
using System.Runtime.Serialization;

namespace App.Service.DomainEntities.Users
{
    [DataContract, Serializable]
    public class ExternalConnection : BaseDomainEntity
    {
        /// <summary>
        /// Represents the id to whom this external connection belongs to
        /// </summary>
        [DataMember]
        public int UserId { get; set; }

        /// <summary>
        /// Name of the external provider
        /// </summary>
        [DataMember]
        public string Provider { get; set; }

        /// <summary>
        /// Access token provided by the external connection
        /// </summary>
        [DataMember]
        public string AccessToken { get; set; }

        /// <summary>
        /// Access token secret provided by the external connection
        /// </summary>
        [DataMember]
        public string AccessTokenSecret { get; set; }

        /// <summary>
        /// External user id
        /// </summary>
        [DataMember]
        public string ExternalUserId { get; set; }

        /// <summary>
        /// External username
        /// </summary>
        [DataMember]
        public string ExternalUsername { get; set; }

        /// <summary>
        /// External data retrieved for the user
        /// </summary>
        [DataMember]
        public string Data { get; set; }
    }
}