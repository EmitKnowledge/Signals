using Ganss.XSS;
using Signals.Core.Common.Instance;
using Signals.Core.Processing.Input;

namespace App.Domain.Processes.Users.Dtos.Request
{
    public class ChangePasswordRequestDto : IDtoData
    {
        /// <summary>
        /// Represents the current password of the user
        /// </summary>
        public string CurrentPassword { get; set; }

        /// <summary>
        /// Represents the new password of the user
        /// </summary>
        public string NewPassword { get; set; }

        /// <summary>
        /// Confirmation of the new password
        /// </summary>
        public string ConfirmPassword { get; set; }

        public void Sanitize(HtmlSanitizer sanitizer)
        {
            CurrentPassword = CurrentPassword.IsNullOrEmpty() ? CurrentPassword : sanitizer.Sanitize(CurrentPassword);
            NewPassword = NewPassword.IsNullOrEmpty() ? NewPassword : sanitizer.Sanitize(NewPassword);
            ConfirmPassword = ConfirmPassword.IsNullOrEmpty() ? ConfirmPassword : sanitizer.Sanitize(ConfirmPassword);
        }
    }
}