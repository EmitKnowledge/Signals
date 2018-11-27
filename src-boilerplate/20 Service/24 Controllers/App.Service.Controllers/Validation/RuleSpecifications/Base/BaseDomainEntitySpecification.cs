using App.Service.DomainEntities.Base;
using Signals.Core.Processing.Specifications;

namespace App.Service.Controllers.Validation.RuleSpecifications.Base
{
    /// <summary>
    /// Validate base entity agains null check and default id check
    /// </summary>
    public class BaseDomainEntitySpecification<T> : BaseSpecification<T> where T : BaseDomainEntity
    {
        #region Implementation of IValidationRule<User>

        /// <summary>
        /// Validation expression that must be fullfilled
        /// </summary>
        /// <returns></returns>
        public override bool Validate(T input)
        {
            return input != null;
        }

        #endregion Implementation of IValidationRule<User>
    }
}