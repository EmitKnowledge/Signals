using App.Domain.Configuration;
using App.Domain.Entities.Users;
using Signals.Aspects.Localization.Helpers;
using Signals.Core.Common.Regexes;
using Signals.Core.Processing.Specifications;

namespace App.Domain.Processes.Users.Specification
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
            if (DomainConfiguration.Instance?.SecurityConfiguration?.MinPasswordLenght != null)
            {
                var _passwordValidityRegex = string.Format(@"^.{{{0},}}$", DomainConfiguration.Instance.SecurityConfiguration.MinPasswordLenght);
                return input.Password.IsMatch(_passwordValidityRegex);
            }

            return true;
        }

        #endregion Implementation of IValidationRule<User>
    }
}