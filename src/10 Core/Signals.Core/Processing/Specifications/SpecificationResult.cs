using System;

namespace Signals.Core.Processing.Specifications
{
    /// <summary>
    /// Specification execution result
    /// </summary>
    public class SpecificationResult
    {
        /// <summary>
        /// Specificaiton executed
        /// </summary>
        public readonly Type SpecificationType;

        /// <summary>
        /// Execution result
        /// </summary>
        public readonly bool IsValid;

        /// <summary>
        /// Input object
        /// </summary>
        public readonly object Input;

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="specificationType"></param>
        /// <param name="isValid"></param>
        /// <param name="input"></param>
        public SpecificationResult(Type specificationType, bool isValid, object input = null)
        {
            SpecificationType = specificationType;
            IsValid = isValid;
            Input = input;
        }
    }
}