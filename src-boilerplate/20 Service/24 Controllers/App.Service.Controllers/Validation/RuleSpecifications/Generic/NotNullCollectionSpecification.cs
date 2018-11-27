﻿using Signals.Core.Processing.Specifications;
using System.Collections.Generic;

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