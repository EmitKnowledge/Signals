using App.Domain.Entities.Users;
using Signals.Core.Common.Instance;
using Signals.Core.Processing.Specifications;

namespace App.Domain.Processes.Users.Specification
{
    /// <summary>
    /// Check if user is not verified
    /// </summary>
    public class NotVerifiedUserSpecification : BaseSpecification<User>
    {
        #region Overrides of BaseSpecification<User>

        /// <summary>
        /// Validation expression that must be fullfilled
        /// </summary>
        /// <returns></returns>
        public override bool Validate(User input)
        {
            return input.IsNull();
        }

        #endregion Overrides of BaseSpecification<User>
    }
}