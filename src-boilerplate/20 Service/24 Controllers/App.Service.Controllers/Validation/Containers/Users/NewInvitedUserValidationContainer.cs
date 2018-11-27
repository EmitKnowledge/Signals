using App.Service.Controllers.Validation.RuleSpecifications.Base;
using App.Service.Controllers.Validation.RuleSpecifications.Users;
using App.Service.DomainEntities.Users;
using Signals.Core.Processing.Specifications;
using System;

namespace App.Service.Controllers.Validation.Containers.Users
{
    public class NewInvitedUserValidationContainer : BaseSpecificationContainer<User>
    {
        public NewInvitedUserValidationContainer(Func<bool> existingUserDelegate)
        {
            this.Add(new BaseDomainEntitySpecification<User>());
            this.Add(new EmailMatchingSpecification());
            this.Add(new IsExistingUserSpecification(existingUserDelegate));
        }
    }
}