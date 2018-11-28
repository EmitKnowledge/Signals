using System;
using System.Runtime.Serialization;

namespace App.Domain.Entities.Users
{
    /// <summary>
    /// Represents the user type (Default roles)
    /// </summary>
    [DataContract]
    [Serializable]
    public enum UserType
    {
        /// <summary>
        /// Anonymous app user
        /// </summary>
        [EnumMember]
        Anonymous,

        /// <summary>
        /// Individual (customer or customer's associate) user of the app
        /// </summary>
        [EnumMember]
        User,

        /// <summary>
        /// Manual worker user for video clip tagging
        /// </summary>
        [EnumMember]
        MechanicalTurk,

        /// <summary>
        /// Administrator of the system
        /// </summary>
        [EnumMember]
        SystemAdministrator
    }
}