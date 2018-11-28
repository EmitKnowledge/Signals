using App.Domain.Configuration;
using Signals.Core.Common.Instance;
using Signals.Core.Processing.Specifications;

namespace App.Domain.Processes.Validation.RuleSpecifications.Tokens
{
    /// <summary>
    /// Check if token exist in token repository
    /// </summary>
    public class IsValidTokenFormatSpecification : BaseSpecification<string>
    {
        #region Overrides of BaseSpecification<ObjectId>

        /// <summary>
        /// Validation expression that must be fullfilled
        /// </summary>
        /// <returns></returns>
        public override bool Validate(string input)
        {
            return input.IsNullOrEmpty() && input.Length == DomainConfiguration.Instance.SecurityConfiguration.TokenLenght;
        }

        #endregion Overrides of BaseSpecification<ObjectId>
    }
}