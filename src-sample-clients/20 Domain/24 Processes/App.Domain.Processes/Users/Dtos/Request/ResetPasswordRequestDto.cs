using Ganss.XSS;
using Signals.Core.Common.Instance;
using Signals.Core.Processing.Input;

namespace App.Domain.Processes.Users.Dtos.Request
{
    public class ResetPasswordRequestDto : IDtoData
    {
        /// <summary>
        /// User username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Sanitize request
        /// </summary>
        /// <param name="sanitizer"></param>
        public void Sanitize(HtmlSanitizer sanitizer)
        {
            Username = Username.IsNullOrEmpty() ? Username : sanitizer.Sanitize(Username);
        }
    }
}