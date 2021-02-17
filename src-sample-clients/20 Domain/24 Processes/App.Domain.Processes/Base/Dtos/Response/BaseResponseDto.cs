using App.Domain.Entities.Base;
using NodaTime;
using System;
using System.Runtime.Serialization;

namespace App.Domain.Processes.Base.Dtos.Response
{
    [DataContract, Serializable]
    public class BaseResponseDto<TEntity> where TEntity : BaseDomainEntity
    {
        /// <summary>
        /// Represents the entity's Id
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// Represents the entity's creation date
        /// </summary>
        [DataMember]
        public Instant CreatedOn { get; set; }

        protected static TEntityDto FromEntity<TEntityDto>(TEntity entity) where TEntityDto : BaseResponseDto<TEntity>, new()
        {
            if (entity == null)
            {
                return null;
            }

            return new TEntityDto
            {
                Id = entity.Id,
                CreatedOn = entity.CreatedOn
            };
        }
    }
}