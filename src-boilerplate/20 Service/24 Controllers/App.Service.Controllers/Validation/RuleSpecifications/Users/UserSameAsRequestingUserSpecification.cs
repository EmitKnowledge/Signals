using System;
using System.Linq.Expressions;
using App.Service.Controllers.Validation.RuleSpecifications.Base;
using App.Service.DomainEntities.Users;
using Signals.Core.Common.Instance;
using Signals.Core.Processing.Specifications;

namespace App.Service.Controllers.Validation.RuleSpecifications.Users
{
    /// <summary>
    /// Validate if the requesting user is the same as user to whom we execute the action
    /// </summary>
    public class UserSameAsRequestingUserSpecification : BaseSpecification<User>
    {
        User RequestingUser { get; set; }

        User User { get; set; }

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

        #endregion
    }
}
