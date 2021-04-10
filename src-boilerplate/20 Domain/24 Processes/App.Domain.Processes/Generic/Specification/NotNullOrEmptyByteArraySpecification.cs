using Signals.Core.Processing.Specifications;

namespace App.Domain.Processes.Generic.Specification
{
    /// <summary>
    /// Validate if byte array is not null and emptu
    /// </summary>
    public class NotNullOrEmptyByteArraySpecification : BaseSpecification<byte[]>
    {
        /// <summary>
        /// Validation expression that must be fullfilled
        /// </summary>
        /// <returns></returns>
        public override bool Validate(byte[] input)
        {
            return input != null && input.Length > 0;
        }
    }
}