using App.Service.DomainEntities.Users;
using Signals.Aspects.Localization.Helpers;
using Signals.Core.Common.Regexes;
using Signals.Core.Processing.Specifications;
using System.Text.RegularExpressions;

namespace App.Service.Controllers.Validation.RuleSpecifications.Users
{
    /// <summary>
    /// Validate user's username
    /// </summary>
    public class UsernameMatchingSpecification : BaseSpecification<User>
    {
        #region Implementation of IValidationRule<User>

        /// <summary>
        /// Validation expression that must be fullfilled
        /// </summary>
        /// <returns></returns>

        public override bool Validate(User input)
        {
            return input.Username.IsMatch(@"^[a-zA-Z0-9]+$", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        }

        #endregion Implementation of IValidationRule<User>
    }
}