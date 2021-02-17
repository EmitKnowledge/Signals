using App.Domain.Entities.Users;
using App.Domain.Processes.Base.Dtos.Response;
using System;
using System.Runtime.Serialization;

namespace App.Domain.Processes.Users.Dtos.Response
{
    [DataContract, Serializable]
    public class GetUserDataResponseDto : BaseUserResponseDto<User>
    {
        /// <summary>
        /// Represents the full name of the user
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Represents the username
        /// </summary>
        [DataMember]
        public string Username { get; set; }

        /// <summary>
        /// Represents the user email
        /// </summary>
        [DataMember]
        public string Email { get; set; }

        /// <summary>
        /// Represents the user role(admin or estimator)
        /// </summary>
        [DataMember]
        public UserType Role { get; set; }

        /// <summary>
        /// Represents the token from the user login
        /// </summary>
        [DataMember]
        public string AuthToken { get; set; }

        /// <summary>
        /// Description of the user
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// Last access date of the user
        /// </summary>
        [DataMember]
        public DateTime? LastAccessDate { get; set; }

        /// <summary>
        /// Is reset password required before user can log in
        /// </summary>
        [DataMember]
        public bool PasswordResetRequired { get; set; }

        public static GetUserDataResponseDto FromEntity(User entity)
        {
            if (entity == null)
            {
                return null;
            }

            var userDto = FromEntity<GetUserDataResponseDto>(entity);

            userDto.Name = entity.Name;
            userDto.Username = entity.Username;
            userDto.Email = entity.Email;
            userDto.Role = entity.Type;
            userDto.AuthToken = entity.Token;
            userDto.LastAccessDate = entity.LastAccessDate;
            userDto.PasswordResetRequired = entity.PasswordResetRequired;

            return userDto;
        }
    }
}