using System;
using System.Runtime.Serialization;

namespace App.Domain.Entities.Users
{
    /// <summary>
    /// Represents login status success
    /// </summary>
    [DataContract]
    [Serializable]
    public enum LoginStatus
    {
        /// <summary>
        /// Failed due to system error
        /// </summary>
        [EnumMember]
        Fail,

        /// <summary>
        /// The provided username/password combination is not valid
        /// </summary>
        [EnumMember]
        UsernameOrPasswordNotValid,

        /// <summary>
        /// Account has been locked by the system administrator
        /// </summary>
        [EnumMember]
        UserAccountIsLocked,

        /// <summary>
        /// The provided username/password combination is valid
        /// </summary>
        [EnumMember]
        Success
    }
}