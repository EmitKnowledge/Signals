using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

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
