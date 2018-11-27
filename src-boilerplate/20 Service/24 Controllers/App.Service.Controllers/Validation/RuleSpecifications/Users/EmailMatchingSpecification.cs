﻿using App.Service.DomainEntities.Users;
using Signals.Aspects.Localization.Helpers;
using Signals.Core.Common.Regexes;
using Signals.Core.Processing.Specifications;
using System;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace App.Service.Controllers.Validation.RuleSpecifications.Users
{
    /// <summary>
    /// Validate user's email
    /// </summary>
    public class EmailMatchingSpecification : BaseSpecification<User>
    {
        #region Implementation of IValidationRule<User>

        /// <summary>
        /// Validation expression that must be fullfilled
        /// </summary>
        /// <returns></returns>
        public override bool Validate(User input)
        {
            return input.Email.IsMatch(@"^([0-9a-zA-Z]([+-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,})$", 
                    RegexOptions.Singleline | 
                    RegexOptions.IgnoreCase | 
                    RegexOptions.ExplicitCapture | 
                    RegexOptions.CultureInvariant);
        }

        #endregion
    }
}
