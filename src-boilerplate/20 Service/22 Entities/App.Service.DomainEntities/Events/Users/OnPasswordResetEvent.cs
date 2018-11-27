using App.Service.DomainEntities.Events.Base;
using System;
using System.Runtime.Serialization;

namespace App.Service.DomainEntities.Events.Users
{
    /// <summary>
    /// Represents token for forgoten password action
    /// </summary>
    [Serializable]
    [DataContract]
    public class OnPasswordResetEvent : BaseSystemTokenEvent { }
}