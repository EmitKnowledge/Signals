using App.Service.Controllers.Validation.RuleSpecifications.Base;
using App.Service.DomainEntities.Users;
using Signals.Core.Processing.Specifications;

namespace App.Service.Controllers.Validation.Containers.Generic
{
    public class RequestingUserValidationContainer : BaseSpecificationContainer<User>
    {
        public RequestingUserValidationContainer()
        {
            this.Add(new BaseDomainEntitySpecification<User>());
        }
    }
}