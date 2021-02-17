using App.Domain.DataRepositoryContracts;
using App.Domain.Entities.Users;
using App.Domain.Processes.Generic.Specification;
using App.Domain.Processes.Users.Dtos.Request;
using App.Domain.Processes.Users.Specification;
using Signals.Aspects.DI.Attributes;
using Signals.Core.Processes.Business;
using Signals.Core.Processing.Results;
using Signals.Core.Processing.Specifications;

namespace App.Domain.Processes.Users
{
    public partial class UserProcesses
    {
        public class EditUser : BusinessProcess<EditUserRequestDto, VoidResult>
        {
            [Import] private IUserRepository UserRepository { get; set; }
            private User User => Context?.Authentication?.GetCurrentUser<User>();

            public override VoidResult Auth(EditUserRequestDto editUserRequestDto)
            {
                return Ok();
            }

            public override VoidResult Validate(EditUserRequestDto editUserRequestDto)
            {
                return BeginValidation()
                        .Validate(new NotNullEntity<EditUserRequestDto>(), editUserRequestDto)
                        .Validate(new NotNullOrEmptyString(), editUserRequestDto.Email)
                        .Validate(new NotNullOrEmptyString(), editUserRequestDto.Name)
                        .Validate(new NotNullOrEmptyString(), editUserRequestDto.Username)
                        .Validate(new ChangeRoleSpecification(User), editUserRequestDto)
                        .UseStrategy(new ExecuteAllStrategy())
                        .Validate(new UniqueOrSameEmailSpecification(editUserRequestDto.Id), editUserRequestDto.Email)
                        .Validate(new UniqueOrSameUsernameSpecification(editUserRequestDto.Id), editUserRequestDto.Username)
                        .Validate(new UsernameMatchingSpecification(), editUserRequestDto.Username)
                        .Validate(new EmailMatchingSpecification(), editUserRequestDto.Email)
                        .ReturnResult();
            }

            public override VoidResult Handle(EditUserRequestDto editUserRequestDto)
            {
                var user = new User();

                user.Name = editUserRequestDto.Name;
                user.Username = editUserRequestDto.Username;
                user.Email = editUserRequestDto.Email;
                user.Type = editUserRequestDto.Type;
                user.Id = editUserRequestDto.Id;

                UserRepository.UpdateUserProfile(user);

                return Ok();
            }
        }
    }
}