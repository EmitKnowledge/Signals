using Signals.Core.Processing.Specifications;

namespace App.Domain.Processes.Users.Specification
{
    internal class ConfirmPasswordCheckSpecification : BaseSpecification<string>
    {
        #region Implementation of IValidationRule<string>

        private string Password { get; set; }

        public ConfirmPasswordCheckSpecification(string password)
        {
            Password = password;
        }

        public override bool Validate(string confirmPassword)
        {
            return Password.Equals(confirmPassword);
        }

        #endregion Implementation of IValidationRule<string>
    }
}