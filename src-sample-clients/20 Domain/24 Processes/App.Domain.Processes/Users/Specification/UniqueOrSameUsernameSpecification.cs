using App.Domain.DataRepositoryContracts;
using Signals.Aspects.DI.Attributes;
using Signals.Core.Common.Instance;
using Signals.Core.Processing.Specifications;

namespace App.Domain.Processes.Users.Specification
{
    /// <summary>
    /// Validate is the username is unique
    /// </summary>
    public class UniqueOrSameUsernameSpecification : BaseSpecification<string>
    {
        #region Implementation of IValidationRule<string>

        [Import] private IUserRepository UserRepository { get; set; }

        private int Id { get; set; }

        public UniqueOrSameUsernameSpecification(int id)
        {
            Id = id;
        }

        /// <summary>
        /// Validation expression that must be fullfilled
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override bool Validate(string input)
        {
            var user = UserRepository.GetUserByUsername(input);
            return user.IsNull() || (!user.IsNull() && user.Id == Id);
        }

        #endregion Implementation of IValidationRule<string>
    }
}