using System;
using System.Runtime.Serialization;
using App.Service.DomainEntities.Events.Base;

namespace App.Service.DomainEntities.Events.Users
{
    /// <summary>
    /// Represents token for user mail confirmation mail
    /// </summary>
    [Serializable]
    [DataContract]
    public class OnNewUserRegisterEvent : BaseSystemTokenEvent { }
}
