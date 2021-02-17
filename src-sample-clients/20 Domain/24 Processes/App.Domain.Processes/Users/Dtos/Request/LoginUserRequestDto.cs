using Ganss.XSS;
using Signals.Core.Common.Instance;
using Signals.Core.Processing.Input;

namespace App.Domain.Processes.Users.Dtos.Request
{
    public class LoginUserRequestDto : IDtoData
    {
        /// <summary>
        /// Represents the username of the user
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Represents the user password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Remember me field
        /// </summary>
        public bool RememberMe { get; set; }

        public void Sanitize(HtmlSanitizer sanitizer)
        {
            Username = Username.IsNullOrEmpty() ? Username : sanitizer.Sanitize(Username);
            Password = Password.IsNullOrEmpty() ? Password : sanitizer.Sanitize(Password);
        }
    }
}