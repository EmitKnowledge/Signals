using App.Service.DomainEntities.Users;
using Signals.Core.Common.Instance;
using Signals.Core.Processing.Specifications;
using System;
using System.Linq.Expressions;

namespace App.Service.Controllers.Validation.RuleSpecifications.Users
{
    /// <summary>
    /// Check if user is not verified
    /// </summary>
    public class NotVerifiedUserSpecification : BaseSpecification<User>
    {
        #region Overrides of BaseSpecification<User>

        /// <summary>
        /// Validation expression that must be fullfilled
        /// </summary>
        /// <returns></returns>
        public override bool Validate(User input)
        {
            return input.IsNull();
        }

        #endregion
    }
}
