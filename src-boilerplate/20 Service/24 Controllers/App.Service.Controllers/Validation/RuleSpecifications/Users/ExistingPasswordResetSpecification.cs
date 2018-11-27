using System;
using System.Linq.Expressions;
using App.Service.Controllers.Validation.RuleSpecifications.Base;
using App.Service.Configuration;
using Signals.Core.Processing.Specifications;
using App.Service.DomainEntities.Events.Users;
using Signals.Core.Common.Instance;
using NodaTime;

namespace App.Service.Controllers.Validation.RuleSpecifications.Users
{
    /// <summary>
    /// Validate that password reset token is valid
    /// </summary>
    public class ExistingPasswordResetSpecification : BaseSpecification<OnPasswordResetEvent>
    {
        #region Implementation of IValidationRule<User>

        /// <summary>
        /// Validation expression that must be fullfilled
        /// </summary>
        /// <returns></returns>
        public override bool Validate(OnPasswordResetEvent input)
        {
            return input != null && input.Id != 0
                    && input.ValidUntil > SystemClock.Instance.GetCurrentInstant()
                    && input.UserId != 0
                    && !input.Token.IsNullOrEmpty()
                    && input.Token.Length == BusinessConfiguration.Instance.SecurityConfiguration.TokenLenght;
        }

        #endregion
    }
}
