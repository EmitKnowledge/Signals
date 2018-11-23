using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Signals.Aspects.Configuration.Validations
{
    public class ValidateObjectAttribute : ValidationAttribute
    {
	    /// <summary>Validates the specified value with respect to the current validation attribute.</summary>
	    /// <param name="value">The value to validate.</param>
	    /// <param name="validationContext">The context information about the validation operation.</param>
	    /// <returns>An instance of the <see cref="System.ComponentModel.DataAnnotations.ValidationResult"></see> class.</returns>
	    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            var results = new List<ValidationResult>();
            var context = new ValidationContext(value, null, null);

            if (value is IEnumerable enumeration)
            {
                foreach (var element in enumeration)
                {
                    var enumContext = new ValidationContext(element, null, null);
                    Validator.TryValidateObject(element, enumContext, results, true);
                }
            }

            Validator.TryValidateObject(value, context, results, true);

            if (results.Count != 0)
            {
                var compositeResults = new CompositeValidationResult($"Validation for {validationContext.DisplayName} failed!");
                results.ForEach(compositeResults.AddResult);

                return compositeResults;
            }

            return ValidationResult.Success;
        }
    }
}
