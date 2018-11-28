using System;
using App.Domain.Entities.Users;
using Signals.Core.Processing.Specifications;

namespace App.Domain.Processes.Users.Specification
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

        #endregion Overrides of BaseSpecification<User>
    }
}