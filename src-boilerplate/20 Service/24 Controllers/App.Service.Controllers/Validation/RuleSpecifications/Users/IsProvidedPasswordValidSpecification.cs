using App.Service.DomainEntities.Users;
using Signals.Core.Processing.Specifications;
using System;

namespace App.Service.Controllers.Validation.RuleSpecifications.Users
{
    /// <summary>
    /// Check if the provided user's password is valid
    /// </summary>
    public class IsProvidedPasswordValidSpecification : BaseSpecification<User>
    {
        public IsProvidedPasswordValidSpecification(Func<bool> passwordValidityCheckDelegate)
        {
            PasswordValidityCheckDelegate = passwordValidityCheckDelegate;
        }

        private Func<bool> PasswordValidityCheckDelegate { get; set; }

        #region Overrides of BaseSpecification<User>

        /// <summary>
        /// Validation expression that must be fullfilled
        /// </summary>
        /// <returns></returns>
        public override bool Validate(User input)
        {
            return PasswordValidityCheckDelegate();
        }

        #endregion Overrides of BaseSpecification<User>
    }
}