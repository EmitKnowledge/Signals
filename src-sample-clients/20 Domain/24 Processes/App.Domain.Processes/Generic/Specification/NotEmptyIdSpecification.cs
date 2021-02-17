using Signals.Core.Processing.Specifications;

namespace App.Domain.Processes.Generic.Specification
{
    /// <summary>
    /// Check if entity id != default id
    /// </summary>
    public class NotEmptyIdSpecification : BaseSpecification<int>
    {
        #region Implementation of IValidationRule<User>

        /// <summary>
        /// Validation expression that must be fullfilled
        /// </summary>
        /// <returns></returns>
        public override bool Validate(int input)
        {
            return input != 0;
        }

        #endregion Implementation of IValidationRule<User>
    }
}