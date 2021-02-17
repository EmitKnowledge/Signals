using App.Domain.Entities.Base;
using System;
using System.Runtime.Serialization;

namespace App.Domain.Entities.Users
{
    /// <summary>
    /// Represent the client/customer user in the system
    /// </summary>
    [Serializable]
    [DataContract]
    //[BsonIgnoreExtraElements]
    public class User : BaseDomainEntity
    {
        /// <summary>
        /// Username
        /// </summary>
        [DataMember]
        public string Username { get; set; }

        /// <summary>
        /// User's hashed pass
        /// </summary>
        [DataMember]
        public string Password { get; set; }

        /// <summary>
        /// Salt used for the encrypting the password
        /// </summary>
        [DataMember]
        public string PasswordSalt { get; set; }

        /// <summary>
        /// Is reset password required before user can log in
        /// </summary>
        [DataMember]
        public bool PasswordResetRequired { get; set; }

        /// <summary>
        /// Email of the user
        /// </summary>
        [DataMember]
        public string Email { get; set; }

        /// <summary>
        /// Name (First name and Last name) of the user
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Represents the type of the user
        /// </summary>
        [DataMember]
        public UserType Type { get; set; }

        /// <summary>
        /// Represents user login token
        /// </summary>
        [DataMember]
        public string Token { get; set; }

        /// <summary>
        /// Represents when the user last accessed the app
        /// </summary>
        [DataMember]
        public DateTime? LastAccessDate { get; set; }

        /// <summary>
        /// Has the user checked remember me
        /// </summary>
        [DataMember]
        public bool RememberMe { get; set; }

        /// <summary>
        /// How many times the estimator tried to log in with wrong password
        /// </summary>
        [DataMember]
        public int LoginAttempts { get; set; }
    }
}