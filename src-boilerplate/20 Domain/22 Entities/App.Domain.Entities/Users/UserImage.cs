﻿using App.Domain.Entities.Base;
using System;
using System.Runtime.Serialization;

//using MongoDB.Bson;
//using MongoDB.Bson.Serialization.Attributes;

namespace App.Domain.Entities.Users
{
    /// <summary>
    /// Represent a user image class
    /// </summary>
    [Serializable]
    [DataContract]
    //[BsonIgnoreExtraElements]
    public class UserImage : BaseDomainEntity
    {
        /// <summary>
        /// Id of the user
        /// </summary>
        [DataMember]
        public int UserId { get; set; }

        /// <summary>
        /// User image
        /// </summary>
        [DataMember]
        public byte[] ImageOriginal { get; set; }

        /// <summary>
        /// User image
        /// </summary>
        [DataMember]
        public byte[] ImageSizeVarationA { get; set; }

        /// <summary>
        /// User image
        /// </summary>
        [DataMember]
        public byte[] ImageSizeVarationB { get; set; }

        /// <summary>
        /// User image
        /// </summary>
        [DataMember]
        public byte[] ImageSizeVarationC { get; set; }

        /// <summary>
        /// CTOR
        /// </summary>
        public UserImage()
        {
            ImageOriginal = new byte[0];
            ImageSizeVarationC = new byte[0];
            ImageSizeVarationB = new byte[0];
            ImageSizeVarationA = new byte[0];
        }
    }
}