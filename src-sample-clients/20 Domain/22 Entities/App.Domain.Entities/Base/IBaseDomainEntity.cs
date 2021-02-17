using System.Runtime.Serialization;

namespace App.Domain.Entities.Base
{
    public interface IBaseDomainEntity<TKey>
    {
        /// <summary>
        /// Entity Id
        /// </summary>
        [DataMember]
        int Id { get; set; }
    }
}