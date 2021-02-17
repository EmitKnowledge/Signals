using App.Domain.DataRepositoryContracts;
using App.Domain.Entities.Users;
using App.Domain.Processes.Users.Dtos.Request;
using Signals.Aspects.DI.Attributes;
using Signals.Core.Processing.Specifications;

namespace App.Domain.Processes.Users.Specification
{
    public class ChangeRoleSpecification : BaseSpecification<EditUserRequestDto>
    {
        #region Implementation of IValidationRule<EditUserRequestDto>

        [Import] private IUserRepository UserRepository { get; set; }
        private User CurrentUser { get; set; }

        public ChangeRoleSpecification(User currentUser)
        {
            CurrentUser = currentUser;
        }

        public override bool Validate(EditUserRequestDto input)
        {
            if (CurrentUser.Id == input.Id)
            {
                var existingUser = UserRepository.GetById(input.Id);
                return existingUser.Type == input.Type;
            }

            return true;
        }

        #endregion Implementation of IValidationRule<EditUserRequestDto>
    }
}