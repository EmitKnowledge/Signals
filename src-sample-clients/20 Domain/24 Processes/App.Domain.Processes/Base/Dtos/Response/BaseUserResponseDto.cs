using App.Domain.Entities.Base;
using System;
using System.Runtime.Serialization;

namespace App.Domain.Processes.Base.Dtos.Response
{
    [DataContract, Serializable]
    public class BaseUserResponseDto<TEntity> : BaseResponseDto<TEntity> where TEntity : BaseDomainEntity
    {
        protected new static TEntityDto FromEntity<TEntityDto>(TEntity entity) where TEntityDto : BaseUserResponseDto<TEntity>, new()
        {
            if (entity == null)
            {
                return null;
            }

            var baseUserEntityDto = BaseResponseDto<TEntity>.FromEntity<TEntityDto>(entity);

            return baseUserEntityDto;
        }
    }
}