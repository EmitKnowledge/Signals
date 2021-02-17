using App.Domain.DataRepositoryContracts;
using Signals.Aspects.DI.Attributes;
using Signals.Core.Common.Instance;
using Signals.Core.Processing.Specifications;

namespace App.Domain.Processes.Users.Specification
{
    public class UniqueEmalSpecification : BaseSpecification<string>
    {
        #region Implementation of IValidationRule<string>

        [Import] private IUserRepository UserRepository { get; set; }

        /// <summary>
        /// Validation expression that must be fullfilled
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override bool Validate(string input)
        {
            var user = UserRepository.GetUserByEmail(input);
            return user.IsNull();
        }

        #endregion Implementation of IValidationRule<string>
    }
}