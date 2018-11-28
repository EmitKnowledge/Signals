using App.Domain.Entities.Users;
using Signals.Core.Common.Instance;
using Signals.Core.Processing.Specifications;

namespace App.Domain.Processes.Users.Specification
{
    /// <summary>
    /// Validate if the requesting user is the same as user to whom we execute the action
    /// </summary>
    public class UserSameAsRequestingUserSpecification : BaseSpecification<User>
    {
        private User RequestingUser { get; set; }

        private User User { get; set; }

        #region Implementation of IValidationRule<User>

        public UserSameAsRequestingUserSpecification(User requestingUser, User user)
        {
            RequestingUser = requestingUser;
            User = user;
        }

        /// <summary>
        /// Validation expression that must be fullfilled
        /// </summary>
        /// <returns></returns>
        public override bool Validate(User input)
        {
            return !RequestingUser.IsNull() &&
                    !User.IsNull() &&
                    RequestingUser.Id != 0 &&
                    RequestingUser.Id == User.Id;
        }

        #endregion Implementation of IValidationRule<User>
    }
}