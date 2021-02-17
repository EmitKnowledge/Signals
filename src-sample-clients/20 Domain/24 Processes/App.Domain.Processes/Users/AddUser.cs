using App.Domain.Configuration;
using App.Domain.DataRepositoryContracts;
using App.Domain.Entities.Emails;
using App.Domain.Entities.Users;
using App.Domain.Processes.Emails;
using App.Domain.Processes.Emails.Dtos.Request;
using App.Domain.Processes.Generic.Specification;
using App.Domain.Processes.Users.Dtos.Email;
using App.Domain.Processes.Users.Dtos.Request;
using App.Domain.Processes.Users.Dtos.Transient;
using App.Domain.Processes.Users.Specification;
using Signals.Aspects.DI.Attributes;
using Signals.Core.Common.Cryptography;
using Signals.Core.Common.Smtp;
using Signals.Core.Processes;
using Signals.Core.Processes.Distributed;
using Signals.Core.Processing.Results;
using Signals.Core.Processing.Specifications;
using System;
using System.Collections.Generic;

namespace App.Domain.Processes.Users
{
    public partial class UserProcesses
    {
        public class AddUser : DistributedProcess<AddUserRequestWithPasswordDto, AddUserRequestDto, MethodResult<AddUserTransientDto>>
        {
            [Import] private IUserRepository UserRepository { get; set; }
            [Import] private ISmtpClient SmtpClient { get; set; }

            /// <summary>
            /// Auth process
            /// </summary>
            /// <param name="addUserDto"></param>
            /// <returns></returns>
            public override MethodResult<AddUserTransientDto> Auth(AddUserRequestDto addUserDto)
            {
                return Ok();
            }

            /// <summary>
            /// Validate process
            /// </summary>
            /// <param name="addUserDto"></param>
            /// <returns></returns>
            public override MethodResult<AddUserTransientDto> Validate(AddUserRequestDto addUserDto)
            {
                return BeginValidation()
                        .Validate(new NotNullEntity<AddUserRequestDto>(), addUserDto)
                        .Validate(new NotNullOrEmptyString(), addUserDto.Email)
                        .Validate(new NotNullOrEmptyString(), addUserDto.Name)
                        .Validate(new NotNullOrEmptyString(), addUserDto.Username)
                        .UseStrategy(new ExecuteAllStrategy())
                        .Validate(new UniqueEmalSpecification(), addUserDto.Email)
                        .Validate(new UniqueUsernameSpecification(), addUserDto.Username)
                        .Validate(new UsernameMatchingSpecification(), addUserDto.Username)
                        .Validate(new EmailMatchingSpecification(), addUserDto.Email)
                        .ReturnResult();
            }

            /// <summary>
            /// Handle process
            /// </summary>
            /// <param name="addUserDto"></param>
            /// <returns></returns>
            public override MethodResult<AddUserTransientDto> Handle(AddUserRequestDto addUserDto)
            {
                var user = new User();

                user.Name = addUserDto.Name;
                user.Username = addUserDto.Username;
                user.Email = addUserDto.Email;
                user.Type = addUserDto.Type;
                user.PasswordResetRequired = true;
                user.LastAccessDate = DateTime.UtcNow;
                user.Token = Hashing.GenerateSalt(DomainConfiguration.Instance.SecurityConfiguration.TokenLength);

                user.PasswordSalt = Hashing.GenerateSalt(DomainConfiguration.Instance.SecurityConfiguration.SaltLength);
                string newPassword = Hashing.GenerateSalt(DomainConfiguration.Instance.SecurityConfiguration.AutoPasswordLength);
                user.Password = Hashing.ToSha256(newPassword, user.PasswordSalt);

                UserRepository.Insert(user);

                var transientData = new AddUserTransientDto();

                transientData.Password = newPassword;

                var insertedUser = UserRepository.GetUserByEmail(user.Email);
                transientData.Id = insertedUser.Id;

                return transientData;
            }

            /// <summary>
            /// Work process
            /// </summary>
            /// <param name="addUserDto"></param>
            /// <returns></returns>
            public override VoidResult Work(AddUserRequestWithPasswordDto addUserDto)
            {
                return Continue<EmailProcesses.SendEmail>().With(new SendEmailRequestDto
                {
                    SendingReason = EmailSendReason.AddUser,
                    SendingReasonKey = $"user-{addUserDto.Id.ToString()}",
                    Template = "AddUser",
                    Model = new AddUserEmailDto
                    {
                        NewPassword = addUserDto.Password,
                        Username = addUserDto.Username,
                        ResetPasswordUrl = DomainConfiguration.Instance.WebConfiguration.Url.Path("login")
                    },
                    To = new List<string> { addUserDto.Email }
                });
            }

            /// <summary>
            /// Map data
            /// </summary>
            /// <param name="request"></param>
            /// <param name="response"></param>
            /// <returns></returns>
            public override AddUserRequestWithPasswordDto Map(AddUserRequestDto request, MethodResult<AddUserTransientDto> response)
            {
                var requestWithPassword = new AddUserRequestWithPasswordDto();

                requestWithPassword.Name = request.Name;
                requestWithPassword.Username = request.Username;
                requestWithPassword.Email = request.Email;
                requestWithPassword.Type = request.Type;
                requestWithPassword.Password = response.Result.Password;
                requestWithPassword.Id = response.Result.Id;

                response.Result = null;

                return requestWithPassword;
            }
        }
    }
}