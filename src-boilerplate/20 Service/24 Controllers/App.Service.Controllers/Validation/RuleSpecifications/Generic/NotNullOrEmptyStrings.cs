using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using App.Service.Controllers.Validation.RuleSpecifications.Base;
using Signals.Core.Common.Instance;
using Signals.Core.Processing.Specifications;

namespace App.Service.Controllers.Validation.RuleSpecifications.Generic
{
    /// <summary>
    /// Validate a list that all elements are not null and that there is at least one element in the array
    /// </summary>
    public class NotNullOrEmptyStrings : BaseSpecification<IEnumerable<string>>
    {
        #region Overrides of BaseSpecification<string>

        /// <summary>
        /// Validation expression that must be fullfilled
        /// </summary>
        /// <returns></returns>
        public override bool Validate(IEnumerable<string> input)
        {
            return input.Count(y => y.IsNullOrEmpty()) == 0;
        }

        #endregion
    }
}
