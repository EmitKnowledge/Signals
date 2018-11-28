using App.Domain.Entities.Events.Base;
using System;
using System.Runtime.Serialization;

namespace App.Domain.Entities.Events.Users
{
    /// <summary>
    /// Represents token for user mail confirmation mail
    /// </summary>
    [Serializable]
    [DataContract]
    public class OnNewUserRegisterEvent : BaseSystemTokenEvent { }
}