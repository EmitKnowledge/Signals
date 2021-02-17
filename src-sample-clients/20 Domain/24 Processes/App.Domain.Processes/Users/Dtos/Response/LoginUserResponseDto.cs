using App.Domain.Entities.Users;
using App.Domain.Processes.Base.Dtos.Response;
using System;
using System.Runtime.Serialization;

namespace App.Domain.Processes.Users.Dtos.Response
{
    [DataContract, Serializable]
    public class LoginUserResponseDto : BaseUserResponseDto<User>
    {
        /// <summary>
        /// Represents the token from the user login
        /// </summary>
        [DataMember]
        public string AuthToken { get; set; }

        public static LoginUserResponseDto FromEntity(User entity)
        {
            if (entity == null)
            {
                return null;
            }

            var userDto = FromEntity<LoginUserResponseDto>(entity);
            userDto.AuthToken = entity.Token;

            return userDto;
        }
    }
}