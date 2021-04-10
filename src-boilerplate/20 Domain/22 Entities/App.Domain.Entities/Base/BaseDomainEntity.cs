using NodaTime;
using System;
using System.Runtime.Serialization;

namespace App.Domain.Entities.Base
{
    /// <summary>
    /// Represents base domain entity
    /// </summary>
    [Serializable]
    [DataContract]
    public class BaseDomainEntity : IBaseDomainEntity<int>
    {
        /// <summary>
        /// Entity Id
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// Indicates when the records have been created in the system
        /// </summary>
        [DataMember]
        public Instant CreatedOn { get; set; }

        /// <summary>
        /// CTOR
        /// </summary>
        public BaseDomainEntity()
        {
            CreatedOn = SystemClock.Instance.GetCurrentInstant();
        }
    }
}