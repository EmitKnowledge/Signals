using App.Domain.DataRepositoryContracts;
using App.Domain.Processes.Generic.Specification;
using App.Domain.Processes.Users.Dtos.Request;
using App.Domain.Processes.Users.Dtos.Response;
using App.Domain.Processes.Users.Specification;
using Signals.Aspects.DI.Attributes;
using Signals.Core.Common.Cryptography;
using Signals.Core.Common.Instance;
using Signals.Core.Processes.Business;
using Signals.Core.Processing.Exceptions;
using Signals.Core.Processing.Results;

namespace App.Domain.Processes.Users
{
    public partial class UserProcesses
    {
        public class LoginUser : BusinessProcess<LoginUserRequestDto, MethodResult<LoginUserResponseDto>>
        {
            [Import] private IUserRepository UserRepository { get; set; }

            /// <summary>
            /// Auth process
            /// </summary>
            /// <param name="loginUserDto"></param>
            /// <returns></returns>
            public override MethodResult<LoginUserResponseDto> Auth(LoginUserRequestDto loginUserDto)
            {
                return Ok();
            }

            /// <summary>
            /// Validate process
            /// </summary>
            /// <param name="loginUserDto"></param>
            /// <returns></returns>
            public override MethodResult<LoginUserResponseDto> Validate(LoginUserRequestDto loginUserDto)
            {
                return BeginValidation()
                    .Validate(new NotNullEntity<LoginUserRequestDto>(), loginUserDto)
                    .Validate(new UserExistsSpecification(), loginUserDto.Username)
                    .ReturnResult();
            }

            /// <summary>
            /// Handle process
            /// </summary>
            /// <param name="loginUserDto"></param>
            /// <returns></returns>
            public override MethodResult<LoginUserResponseDto> Handle(LoginUserRequestDto loginUserDto)
            {
                var user = UserRepository.GetUserByUsername(loginUserDto.Username);
                if (user.IsNull())
                    return Fail(new GeneralErrorInfo("LoginUser_UserNotFound"));

                if (user.LoginAttempts == 3)
                {
                    return Fail(new GeneralErrorInfo("LoginUser_ResetPassword"));
                }

                var hashedPassword = user.Password;
                var salt = user.PasswordSalt;

                if (!Hashing.VerifySha256(hashedPassword, loginUserDto.Password, salt))
                {
                    UserRepository.UpdateLoginAttempts(user.Id, user.LoginAttempts + 1);
                    return Fail(new GeneralErrorInfo("LoginUser_InvalidCredentials"));
                }

                UserRepository.UpdateLoginAttempts(user.Id, 0);
                UserRepository.UpdateLastAccessDateAndRememberMe(user.Token, loginUserDto.RememberMe);

                return LoginUserResponseDto.FromEntity(user);
            }
        }
    }
}