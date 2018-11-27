﻿using System;
using System.Linq.Expressions;
using App.Service.Controllers.Validation.RuleSpecifications.Base;
using App.Service.DomainEntities.Users;
using Signals.Core.Processing.Specifications;

namespace App.Service.Controllers.Validation.RuleSpecifications.Users
{
    /// <summary>
    /// Validate user id != default id
    /// </summary>
    public class UserIdMatchingSpecification : BaseSpecification<User>
    {
        #region Implementation of IValidationRule<User>

        /// <summary>
        /// Validation expression that must be fullfilled
        /// </summary>
        /// <returns></returns>
        public override bool Validate(User input)
        {
            return input.Id != 0;
        }

        #endregion
    }
}
