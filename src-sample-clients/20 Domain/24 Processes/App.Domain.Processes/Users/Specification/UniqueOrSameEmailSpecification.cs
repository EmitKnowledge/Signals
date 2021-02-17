using App.Domain.DataRepositoryContracts;
using Signals.Aspects.DI.Attributes;
using Signals.Core.Common.Instance;
using Signals.Core.Processing.Specifications;

namespace App.Domain.Processes.Users.Specification
{
    public class UniqueOrSameEmailSpecification : BaseSpecification<string>
    {
        #region Implementation of IValidationRule<string>

        [Import] private IUserRepository UserRepository { get; set; }

        private int Id { get; set; }

        public UniqueOrSameEmailSpecification(int id)
        {
            Id = id;
        }

        /// <summary>
        /// Validation expression that must be fullfilled
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override bool Validate(string input)
        {
            var user = UserRepository.GetUserByEmail(input);
            return user.IsNull() || (!user.IsNull() && user.Id == Id);
        }

        #endregion Implementation of IValidationRule<string>
    }
}