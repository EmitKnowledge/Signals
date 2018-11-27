using App.Service.DomainEntities.Users;
using Signals.Core.Processing.Specifications;
using System;
using System.Linq.Expressions;

namespace App.Service.Controllers.Validation.RuleSpecifications.Users
{
    /// <summary>
    /// Check if a user is existing in database
    /// </summary>
    public class IsExistingUserSpecification : BaseSpecification<User>
    {
        public IsExistingUserSpecification(Func<bool> existingUserDelegate)
        {
            ExistingUserDelegate = existingUserDelegate;
        }

        private Func<bool> ExistingUserDelegate { get; set; }

        #region Overrides of BaseSpecification<User>

        /// <summary>
        /// Validation expression that must be fullfilled
        /// </summary>
        /// <returns></returns>
        public override bool Validate(User input)
        {
            return !ExistingUserDelegate();
        }

        #endregion
    }
}
