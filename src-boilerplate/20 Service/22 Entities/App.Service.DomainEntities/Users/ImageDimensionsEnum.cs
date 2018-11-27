using System;
using System.Runtime.Serialization;

namespace App.Service.DomainEntities.Users
{
    /// <summary>
    /// Represent the size of the profile picture
    /// </summary>
    [DataContract]
    [Serializable]
    public enum UserImageDimensionsEnum
    {
        [EnumMember]
        ImageOriginal,

        [EnumMember]
        ImageSizeVarationA,

        [EnumMember]
        ImageSizeVarationB,

        [EnumMember]
        ImageSizeVarationC
    }
}