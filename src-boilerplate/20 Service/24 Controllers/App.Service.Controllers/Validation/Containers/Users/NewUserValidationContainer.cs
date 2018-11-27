using System;
using App.Service.Controllers.Validation.RuleSpecifications.Base;
using App.Service.Controllers.Validation.RuleSpecifications.Users;
using App.Service.DomainEntities.Users;
using Signals.Core.Processing.Specifications;

namespace App.Service.Controllers.Validation.Containers.Users
{
    public class NewUserValidationContainer : BaseSpecificationContainer<User>
    {
        public NewUserValidationContainer(Func<bool> existingUserDelegate)
        {
            this.Add(new BaseDomainEntitySpecification<User>());
            this.Add(new EmailMatchingSpecification());
            this.Add(new IsExistingUserSpecification(existingUserDelegate));
        }
    }
}
