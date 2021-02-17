using Ganss.XSS;
using Signals.Core.Common.Instance;
using Signals.Core.Processing.Input;

namespace App.Domain.Processes.Users.Dtos.Request
{
    public class ResetPasswordWithProfileDto : IDtoData, ITransientData
    {
        /// <summary>
        /// User id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// User username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// User Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// User password
        /// </summary>
        public string Password { get; set; }

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