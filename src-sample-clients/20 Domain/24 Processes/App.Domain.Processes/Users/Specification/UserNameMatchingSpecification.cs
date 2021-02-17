using Signals.Core.Common.Regexes;
using Signals.Core.Processing.Specifications;
using System.Text.RegularExpressions;

namespace App.Domain.Processes.Users.Specification
{
    /// <summary>
    /// Validate user's username
    /// </summary>
    public class UsernameMatchingSpecification : BaseSpecification<string>
    {
        #region Implementation of IValidationRule<string>

        /// <summary>
        /// Validation expression that must be fullfilled
        /// </summary>
        /// <returns></returns>

        public override bool Validate(string input)
        {
            var usernameValidity = string.Format(@"^(?=.*?[A-Za-z0-9])(?=.*?[#?!@$%^&*-]*).*$");
            return input.IsMatch(usernameValidity, RegexOptions.Singleline | RegexOptions.IgnoreCase);
        }

        #endregion Implementation of IValidationRule<string>
    }
}