using Signals.Core.Processing.Specifications;
using System.Collections.Generic;

namespace App.Domain.Processes.Generic.Specification
{
    /// <summary>
    /// Validate a list that all elements are not null and validate that the list is not empty
    /// </summary>
    public class NotNullOrEmptyCollectionSpecification<T> : BaseSpecification<List<T>>
    {
        /// <summary>
        /// Validation expression that must be fullfilled
        /// </summary>
        /// <returns></returns>
        public override bool Validate(List<T> input)
        {
            return input != null && input.Count > 0;
        }
    }
}