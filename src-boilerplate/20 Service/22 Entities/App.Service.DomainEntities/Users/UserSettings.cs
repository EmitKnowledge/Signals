using System;
using System.Runtime.Serialization;
using App.Service.DomainEntities.Base;

namespace App.Service.DomainEntities.Users
{
    /// <summary>
    /// Represents user settings for particular user
    /// </summary>
    [Serializable]
    [DataContract]
    [KnownType(typeof(DefaultUserSettings))]
    //[BsonIgnoreExtraElements]
    public class UserSettings : BaseDomainEntity
    {
        /// <summary>
        /// Id of the user to whom this settings belongs to
        /// </summary>
        [DataMember]
        public int UserId { get; set; }

        [DataMember]
        public bool SubscribeToProductEmails { get; set; }

        /// <summary>
        /// CTOR
        /// </summary>
        public UserSettings()
        {
            SubscribeToProductEmails = true;
        }
    }

    /// <summary>
    /// Represents default user settings
    /// </summary>
    [Serializable]
    [DataContract]
    //[BsonIgnoreExtraElements]
    public class DefaultUserSettings : UserSettings
    {
        public DefaultUserSettings()
        {
            SubscribeToProductEmails = true;
        }
    }
}
