using App.Domain.DataRepositoryContracts;
using Signals.Aspects.DI.Attributes;
using Signals.Core.Common.Instance;
using Signals.Core.Processing.Specifications;

namespace App.Domain.Processes.Users.Specification
{
    /// <summary>
    /// Check if a user is existing in database
    /// </summary>
    public class UserExistsSpecification : BaseSpecification<string>
    {
        [Import] private IUserRepository UserRepository { get; set; }

        /// <summary>
        /// Validation expression that must be fullfilled
        /// </summary>
        /// <returns></returns>
        public override bool Validate(string input)
        {
            return !UserRepository.GetUserByEmailOrUsername(input).IsNull();
        }
    }
}