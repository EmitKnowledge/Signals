using System;
using System.Linq.Expressions;
using App.Service.Configuration;
using App.Service.DomainEntities.Users;
using Signals.Aspects.Localization.Helpers;
using Signals.Core.Processing.Specifications;

namespace App.Service.Controllers.Validation.RuleSpecifications.Users
{
    /// <summary>
    /// Validate user's password
    /// </summary>
    public class PasswordMatchingSpecification : BaseSpecification<User>
    {
        #region Implementation of IValidationRule<User>

        /// <summary>
        /// Validation expression that must be fullfilled
        /// </summary>
        /// <returns></returns>
        public override bool Validate(User input)
        {
            if (BusinessConfiguration.Instance?.SecurityConfiguration?.MinPasswordLenght != null)
            {
                var _passwordValidityRegex = string.Format(@"^.{{{0},}}$", BusinessConfiguration.Instance.SecurityConfiguration.MinPasswordLenght);
                return input.Password.IsMatch(_passwordValidityRegex);
            }

            return true;
        }

        #endregion
    }
}
