using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Signals.Aspects.Configuration.Validations
{
    public class CompositeValidationResult : ValidationResult
    {
		/// <summary>
		/// Return the results as IEnumerable
		/// </summary>
		public List<ValidationResult> Results { get; } = new List<ValidationResult>();

		/// <summary>
		/// CTOR
		/// </summary>
		/// <param name="errorMessage"></param>
		public CompositeValidationResult(string errorMessage) : base(errorMessage) { }

		/// <summary>
		/// CTOR
		/// </summary>
		/// <param name="errorMessage"></param>
		/// <param name="memberNames"></param>
        public CompositeValidationResult(string errorMessage, IEnumerable<string> memberNames) : base(errorMessage, memberNames) { }
		
		/// <summary>
		/// CTOR
		/// </summary>
		/// <param name="validationResult"></param>
        protected CompositeValidationResult(ValidationResult validationResult) : base(validationResult) { }

		/// <summary>
		/// Add validation result
		/// </summary>
		/// <param name="validationResult"></param>
        public void AddResult(ValidationResult validationResult)
        {
            Results.Add(validationResult);
        }
    }
}
