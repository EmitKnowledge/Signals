using Signals.Core.Processing.Specifications;
using System;

namespace App.Domain.Processes.Users.Specification
{
    /// <summary>
    /// Check if token exist in token repository
    /// </summary>
    public class IsExistingTokenSpecification : BaseSpecification<int>
    {
        #region Overrides of BaseSpecification<ObjectId>

        public IsExistingTokenSpecification(Func<bool> isExistingTokenDelegate)
        {
            IsExistingTokenDelegate = isExistingTokenDelegate;
        }

        private Func<bool> IsExistingTokenDelegate { get; set; }

        /// <summary>
        /// Validation expression that must be fullfilled
        /// </summary>
        /// <returns></returns>
        public override bool Validate(int input)
        {
            return IsExistingTokenDelegate();
        }

        #endregion Overrides of BaseSpecification<ObjectId>
    }
}