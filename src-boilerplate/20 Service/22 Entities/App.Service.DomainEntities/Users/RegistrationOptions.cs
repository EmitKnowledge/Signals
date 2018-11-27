using System;
using System.Runtime.Serialization;

namespace App.Service.DomainEntities.Users
{
    /// <summary>
    /// Represents registration options used for registering new users
    /// </summary>
    [Serializable]
    [DataContract]
    public class RegistrationOptions
    {
        /// <summary>
        /// Indicate if email should be sent upon registration
        /// Default value = True
        /// </summary>
        [DataMember]
        public bool ShouldSendRegistrationEmail { get; set; }

        /// <summary>
        /// Default registration options
        /// </summary>
        [IgnoreDataMember]
        public static readonly RegistrationOptions Default;

        /// <summary>
        /// STATIC CTOR
        /// </summary>
        static RegistrationOptions()
        {
            Default = new RegistrationOptions
            {
                ShouldSendRegistrationEmail = true
            };
        }
    }
}