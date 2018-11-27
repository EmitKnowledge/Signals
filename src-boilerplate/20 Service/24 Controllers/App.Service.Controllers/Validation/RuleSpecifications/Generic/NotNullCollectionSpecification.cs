using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using App.Service.Controllers.Validation.RuleSpecifications.Base;
using Signals.Core.Processing.Specifications;

namespace App.Service.Controllers.Validation.RuleSpecifications.Generic
{
    /// <summary>
    /// Validate all list elements != null
    /// </summary>
    public class NotNullCollectionSpecification<T> : BaseSpecification<List<T>>
    {
        /// <summary>
        /// Validation expression that must be fullfilled
        /// </summary>
        /// <returns></returns>
        public override bool Validate(List<T> input)
        {
            return input != null;
        }
    }
}
