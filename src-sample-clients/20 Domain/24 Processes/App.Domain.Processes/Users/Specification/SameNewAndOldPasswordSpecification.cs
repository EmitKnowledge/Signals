using Signals.Core.Processing.Specifications;

namespace App.Domain.Processes.Users.Specification
{
    public class SameNewAndOldPasswordSpecification : BaseSpecification<string>
    {
        #region Implementation of IValidationRule<string>

        private string CurrentPassword { get; set; }

        public SameNewAndOldPasswordSpecification(string currentPassword)
        {
            CurrentPassword = currentPassword;
        }

        public override bool Validate(string newPassword)
        {
            return newPassword != CurrentPassword;
        }

        #endregion Implementation of IValidationRule<string>
    }
}