using App.Domain.DataRepositoryContracts;
using App.Domain.Entities.Users;
using App.Domain.Processes.Generic.Specification;
using App.Domain.Processes.Users.Dtos.Request;
using App.Domain.Processes.Users.Specification;
using Signals.Aspects.DI.Attributes;
using Signals.Core.Processes.Business;
using Signals.Core.Processing.Results;

namespace App.Domain.Processes.Users
{
    public partial class UserProcesses
    {
        public class DeleteUser : BusinessProcess<DeleteUserRequestDto, VoidResult>
        {
            private User User => Context?.Authentication?.GetCurrentUser<User>();
            [Import] private IUserRepository UserRepository { get; set; }

            /// <summary>
            /// Auth process
            /// </summary>
            /// <param name="deleteUserRequestDto"></param>
            /// <returns></returns>
            public override VoidResult Auth(DeleteUserRequestDto deleteUserRequestDto)
            {
                return Ok();
            }

            /// <summary>
            /// Validate process
            /// </summary>
            /// <param name="deleteUserRequestDto"></param>
            /// <returns></returns>
            public override VoidResult Validate(DeleteUserRequestDto deleteUserRequestDto)
            {
                return BeginValidation()
                        .Validate(new NotNullEntity<DeleteUserRequestDto>(), deleteUserRequestDto)
                        .Validate(new SameUserSpecification(User.Id), deleteUserRequestDto.Id)
                        .ReturnResult();
            }

            /// <summary>
            /// Handle process
            /// </summary>
            /// <param name="deleteUserRequestDto"></param>
            /// <returns></returns>
            public override VoidResult Handle(DeleteUserRequestDto deleteUserRequestDto)
            {
                UserRepository.DeleteById(deleteUserRequestDto.Id);

                return Ok();
            }
        }
    }
}