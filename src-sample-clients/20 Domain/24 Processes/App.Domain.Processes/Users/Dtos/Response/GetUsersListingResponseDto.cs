using App.Domain.Entities.Users;
using App.Domain.Processes.Base.Dtos.Response;
using System;
using System.Runtime.Serialization;

namespace App.Domain.Processes.Users.Dtos.Response
{
    [DataContract, Serializable]
    public class GetUsersListingResponseDto : BaseUserResponseDto<User>
    {
        /// <summary>
        /// Username of the user
        /// </summary>
        [DataMember]
        public string Username { get; set; }

        /// <summary>
        /// Full name of the user
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// User Type
        /// </summary>
        [DataMember]
        public UserType Type { get; set; }

        /// <summary>
        /// Email of the user
        /// </summary>
        [DataMember]
        public string Email { get; set; }

        public static GetUsersListingResponseDto FromEntity(User entity)
        {
            if (entity == null)
            {
                return null;
            }

            var userDto = FromEntity<GetUsersListingResponseDto>(entity);

            userDto.Id = entity.Id;
            userDto.Username = entity.Username;
            userDto.Email = entity.Email;
            userDto.Name = entity.Name;
            userDto.Type = entity.Type;

            return userDto;
        }
    }
}