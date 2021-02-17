using Signals.Core.Common.Regexes;
using Signals.Core.Processing.Specifications;
using System.Text.RegularExpressions;

namespace App.Domain.Processes.Users.Specification
{
    /// <summary>
    /// Validate user's email
    /// </summary>
    public class EmailMatchingSpecification : BaseSpecification<string>
    {
        #region Implementation of IValidationRule<string>

        /// <summary>
        /// Validation expression that must be fullfilled
        /// </summary>
        /// <returns></returns>
        public override bool Validate(string input)
        {
            return input.IsMatch(@"^([0-9a-zA-Z]([+-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,})$",
                    RegexOptions.Singleline |
                    RegexOptions.IgnoreCase |
                    RegexOptions.ExplicitCapture |
                    RegexOptions.CultureInvariant);
        }

        #endregion Implementation of IValidationRule<string>
    }
}