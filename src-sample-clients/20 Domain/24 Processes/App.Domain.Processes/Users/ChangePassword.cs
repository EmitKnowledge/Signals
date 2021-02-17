using App.Domain.Configuration;
using App.Domain.DataRepositoryContracts;
using App.Domain.Entities.Users;
using App.Domain.Processes.Generic.Specification;
using App.Domain.Processes.Users.Dtos.Request;
using App.Domain.Processes.Users.Specification;
using Signals.Aspects.DI.Attributes;
using Signals.Core.Common.Cryptography;
using Signals.Core.Processes.Business;
using Signals.Core.Processing.Exceptions;
using Signals.Core.Processing.Results;
using Signals.Core.Processing.Specifications;

namespace App.Domain.Processes.Users
{
    public partial class UserProcesses
    {
        public class ChangePassword : BusinessProcess<ChangePasswordRequestDto, VoidResult>
        {
            private User User => Context?.Authentication?.GetCurrentUser<User>();
            [Import] private IUserRepository UserRepository { get; set; }

            /// <summary>
            /// Auth process
            /// </summary>
            /// <param name="changePasswordRequestDto"></param>
            /// <returns></returns>
            public override VoidResult Auth(ChangePasswordRequestDto changePasswordRequestDto)
            {
                return Ok();
            }

            /// <summary>
            /// Validation process
            /// </summary>
            /// <param name="changePasswordRequestDto"></param>
            /// <returns></returns>
            public override VoidResult Validate(ChangePasswordRequestDto changePasswordRequestDto)
            {
                return BeginValidation()
                    .Validate(new NotNullOrEmptyString(), changePasswordRequestDto.NewPassword)
                    .Validate(new NotNullOrEmptyString(), changePasswordRequestDto.CurrentPassword)
                    .Validate(new NotNullOrEmptyString(), changePasswordRequestDto.ConfirmPassword)
                    .UseStrategy(new ExecuteAllStrategy())
                    .Validate(new PasswordMatchingSpecification(), changePasswordRequestDto.NewPassword)
                    .Validate(new ConfirmPasswordCheckSpecification(changePasswordRequestDto.NewPassword), changePasswordRequestDto.ConfirmPassword)
                    .Validate(new SameNewAndOldPasswordSpecification(changePasswordRequestDto.CurrentPassword), changePasswordRequestDto.NewPassword)
                    .ReturnResult();
            }

            /// <summary>
            /// Handle process
            /// </summary>
            /// <param name="changePasswordRequestDto"></param>
            /// <returns></returns>
            public override VoidResult Handle(ChangePasswordRequestDto changePasswordRequestDto)
            {
                var user = UserRepository.GetById(User.Id);

                var hashedPassword = user.Password;
                var salt = user.PasswordSalt;
                if (!Hashing.VerifySha256(hashedPassword, changePasswordRequestDto.CurrentPassword, salt))
                    return Fail(new GeneralErrorInfo("ChangePassword_PasswordNotMatchExisting"));

                user.PasswordSalt = Hashing.GenerateSalt(DomainConfiguration.Instance.SecurityConfiguration.SaltLength);
                user.Password = Hashing.ToSha256(changePasswordRequestDto.NewPassword, user.PasswordSalt);

                UserRepository.UpdatePassword(user.Id, user.Password, user.PasswordSalt);

                return Ok();
            }
        }
    }
}