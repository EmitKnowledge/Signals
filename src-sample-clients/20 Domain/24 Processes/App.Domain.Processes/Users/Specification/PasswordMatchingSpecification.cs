using App.Domain.Configuration;
using Signals.Aspects.Localization.Helpers;
using Signals.Core.Common.Regexes;
using Signals.Core.Processing.Specifications;
using System.Text.RegularExpressions;

namespace App.Domain.Processes.Users.Specification
{
    /// <summary>
    /// Validate user's password
    /// </summary>
    public class PasswordMatchingSpecification : BaseSpecification<string>
    {
        #region Implementation of IValidationRule<User>

        /// <summary>
        /// Validation expression that must be fullfilled
        /// </summary>
        /// <returns></returns>
        public override bool Validate(string input)
        {
            if (DomainConfiguration.Instance?.SecurityConfiguration?.MinPasswordLength != null)
            {
                var passwordValidity = string.Format(@"^(?=.*[a-za-zA-Z])(?=.*\d)(?=.*[#$^+=!*()@%&]).{{{0},}}$",
                                                        DomainConfiguration.Instance.SecurityConfiguration.MinPasswordLength);
                return input.IsMatch(passwordValidity,
                                RegexOptions.Singleline |
                                RegexOptions.IgnoreCase |
                                RegexOptions.ExplicitCapture |
                                RegexOptions.CultureInvariant);
            }
            else
            {
                return input.IsMatch(@"^(?=.*[a-za-zA-Z])(?=.*\d)(?=.*[#$^+=!*()@%&])$",
                                RegexOptions.Singleline |
                                RegexOptions.IgnoreCase |
                                RegexOptions.ExplicitCapture |
                                RegexOptions.CultureInvariant);
            }
        }

        #endregion Implementation of IValidationRule<User>
    }
}