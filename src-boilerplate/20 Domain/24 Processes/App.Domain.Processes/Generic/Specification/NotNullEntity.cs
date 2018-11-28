using Signals.Core.Common.Instance;
using Signals.Core.Processing.Specifications;

namespace App.Domain.Processes.Generic.Specification
{
    /// <summary>
    /// Validate if entity is != null
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NotNullEntity<T> : BaseSpecification<T> where T : class
    {
        #region Overrides of BaseSpecification<T>

        /// <summary>
        /// Validation expression that must be fullfilled
        /// </summary>
        /// <returns></returns>
        public override bool Validate(T input)
        {
            return input.IsNull();
        }

        #endregion Overrides of BaseSpecification<T>
    }
}