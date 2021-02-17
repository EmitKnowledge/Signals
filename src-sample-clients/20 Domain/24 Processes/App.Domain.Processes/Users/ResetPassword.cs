using App.Domain.Configuration;
using App.Domain.DataRepositoryContracts;
using App.Domain.Entities.Emails;
using App.Domain.Processes.Emails;
using App.Domain.Processes.Emails.Dtos.Request;
using App.Domain.Processes.Generic.Specification;
using App.Domain.Processes.Users.Dtos.Email;
using App.Domain.Processes.Users.Dtos.Request;
using App.Domain.Processes.Users.Dtos.Transient;
using App.Domain.Processes.Users.Specification;
using Signals.Aspects.DI.Attributes;
using Signals.Core.Common.Cryptography;
using Signals.Core.Common.Instance;
using Signals.Core.Common.Smtp;
using Signals.Core.Processes;
using Signals.Core.Processes.Distributed;
using Signals.Core.Processing.Results;
using System.Collections.Generic;

namespace App.Domain.Processes.Users
{
    public partial class UserProcesses
    {
        public class ResetPassword : DistributedProcess<ResetPasswordWithProfileDto, ResetPasswordRequestDto, MethodResult<ResetPasswordTransientDto>>
        {
            [Import] private IUserRepository UserRepository { get; set; }
            [Import] private ISmtpClient SmtpClient { get; set; }

            /// <summary>
            /// Auth process
            /// </summary>
            /// <param name="resetPasswordRequestDto"></param>
            /// <returns></returns>
            public override MethodResult<ResetPasswordTransientDto> Auth(ResetPasswordRequestDto resetPasswordRequestDto)
            {
                return Ok();
            }

            /// <summary>
            /// Validation process
            /// </summary>
            /// <param name="resetPasswordRequestDto"></param>
            /// <returns></returns>
            public override MethodResult<ResetPasswordTransientDto> Validate(ResetPasswordRequestDto resetPasswordRequestDto)
            {
                return BeginValidation()
                    .Validate(new NotNullEntity<ResetPasswordRequestDto>(), resetPasswordRequestDto)
                    .Validate(new NotNullOrEmptyString(), resetPasswordRequestDto.Username)
                    .Validate(new UserExistsSpecification(), resetPasswordRequestDto.Username)
                    .ReturnResult();
            }

            /// <summary>
            /// Handle process
            /// </summary>
            /// <param name="resetPasswordRequestDto"></param>
            /// <returns></returns>
            public override MethodResult<ResetPasswordTransientDto> Handle(ResetPasswordRequestDto resetPasswordRequestDto)
            {
                var user = UserRepository.GetUserByEmailOrUsername(resetPasswordRequestDto.Username);

                string newPassword = string.Empty;
                while (newPassword.IsNullOrEmpty() && !Hashing.VerifySha256(user.Password, newPassword, user.PasswordSalt))
                {
                    newPassword = Hashing.GenerateSalt(DomainConfiguration.Instance.SecurityConfiguration.AutoPasswordLength);
                }

                user.Password = Hashing.ToSha256(newPassword, user.PasswordSalt);

                UserRepository.UpdatePasswordAndResetLoginAttempts(user.Id, user.Password, user.PasswordSalt, true);

                var transientData = new ResetPasswordTransientDto();

                transientData.NewPassword = newPassword;
                transientData.Id = user.Id;
                transientData.Username = user.Username;
                transientData.Email = user.Email;

                return transientData;
            }

            /// <summary>
            /// Execute on background
            /// </summary>
            /// <param name="request"></param>
            /// <returns></returns>
            public override VoidResult Work(ResetPasswordWithProfileDto request)
            {
                return Continue<EmailProcesses.SendEmail>().With(new SendEmailRequestDto
                {
                    SendingReason = EmailSendReason.UserResetPassword,
                    SendingReasonKey = $"user-{request.Id.ToString()}",
                    Template = "ResetPassword",
                    Model = new ResetPasswordEmailDto
                    {
                        NewPassword = request.Password,
                        Username = request.Username,
                        ResetPasswordUrl = DomainConfiguration.Instance.WebConfiguration.Url.Path("login")
                    },
                    To = new List<string> { request.Email }
                });
            }

            /// <summary>
            /// Map data
            /// </summary>
            /// <param name="request"></param>
            /// <param name="response"></param>
            /// <returns></returns>
            public override ResetPasswordWithProfileDto Map(ResetPasswordRequestDto request, MethodResult<ResetPasswordTransientDto> response)
            {
                var resetWithProfile = new ResetPasswordWithProfileDto();

                resetWithProfile.Id = response.Result.Id;
                resetWithProfile.Username = response.Result.Username;
                resetWithProfile.Email = response.Result.Email;
                resetWithProfile.Password = response.Result.NewPassword;

                response.Result = null;

                return resetWithProfile;
            }
        }
    }
}