using App.Domain.Entities.Users;
using Signals.Core.Processing.Specifications;

namespace App.Domain.Processes.Users.Specification
{
    /// <summary>
    /// Validate user id != default id
    /// </summary>
    public class UserIdMatchingSpecification : BaseSpecification<User>
    {
        #region Implementation of IValidationRule<User>

        /// <summary>
        /// Validation expression that must be fullfilled
        /// </summary>
        /// <returns></returns>
        public override bool Validate(User input)
        {
            return input.Id != 0;
        }

        #endregion Implementation of IValidationRule<User>
    }
}