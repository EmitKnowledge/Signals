using System;
using System.Runtime.Serialization;
using App.Service.DomainEntities.Events.Base;

namespace App.Service.DomainEntities.Events.Users
{
    /// <summary>
    /// Represents token for forgoten password action
    /// </summary>
    [Serializable]
    [DataContract]
    public class OnPasswordResetEvent : BaseSystemTokenEvent { }
}
