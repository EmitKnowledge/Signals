using App.Domain.Entities.Events.Base;
using System;
using System.Runtime.Serialization;

namespace App.Domain.Entities.Events.Users
{
    /// <summary>
    /// Represents token for forgoten password action
    /// </summary>
    [Serializable]
    [DataContract]
    public class OnPasswordResetEvent : BaseSystemTokenEvent { }
}