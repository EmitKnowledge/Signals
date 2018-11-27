using App.Service.DomainEntities.Users;
using Signals.Core.Common.Instance;
using Signals.Core.Processing.Specifications;

namespace App.Service.Controllers.Validation.RuleSpecifications.Users
{
    /// <summary>
    /// Validate user's profile picture
    /// </summary>
    public class UserImageSpecification : BaseSpecification<UserImage>
    {
        #region Implementation of IValidationRule<User>

        /// <summary>
        /// Validation expression that must be fullfilled
        /// </summary>
        /// <returns></returns>
        public override bool Validate(UserImage input)
        {
            return input.UserId != 0 &&
                    !input.ImageOriginal.IsNullOrHasZeroElements() &&
                    !input.ImageSizeVarationA.IsNullOrHasZeroElements() &&
                    !input.ImageSizeVarationB.IsNullOrHasZeroElements() &&
                    !input.ImageSizeVarationC.IsNullOrHasZeroElements();
        }

        #endregion Implementation of IValidationRule<User>
    }
}