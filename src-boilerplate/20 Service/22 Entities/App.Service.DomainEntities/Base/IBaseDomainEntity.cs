using System.Runtime.Serialization;

namespace App.Service.DomainEntities.Base
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