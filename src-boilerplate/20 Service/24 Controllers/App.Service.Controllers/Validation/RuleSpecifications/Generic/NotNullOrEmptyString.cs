using Signals.Core.Common.Instance;
using Signals.Core.Processing.Specifications;
using System;
using System.Linq.Expressions;

namespace App.Service.Controllers.Validation.RuleSpecifications.Generic
{
    /// <summary>
    /// Validate that a string is not empty
    /// </summary>
    public class NotNullOrEmptyString : BaseSpecification<string>
    {
        #region Overrides of BaseSpecification<string>

        /// <summary>
        /// Validation expression that must be fullfilled
        /// </summary>
        /// <returns></returns>
        public override bool Validate(string input)
        {

            return !input.IsNullOrEmpty();
        }

        #endregion
    }
}
