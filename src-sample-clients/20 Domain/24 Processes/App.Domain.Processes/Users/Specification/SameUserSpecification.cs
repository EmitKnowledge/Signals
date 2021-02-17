using Signals.Core.Processing.Specifications;

namespace App.Domain.Processes.Users.Specification
{
    /// <summary>
    /// Check if user is trying to edit his role or delete himself
    /// </summary>
    public class SameUserSpecification : BaseSpecification<int>
    {
        #region Implementation of IValidationRule<int>

        private int CurrentUserId { get; set; }

        public SameUserSpecification(int currentUserId)
        {
            CurrentUserId = currentUserId;
        }

        public override bool Validate(int userId)
        {
            return userId != CurrentUserId;
        }

        #endregion Implementation of IValidationRule<int>
    }
}