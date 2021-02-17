using Ganss.XSS;
using Signals.Core.Processing.Input;

namespace App.Domain.Processes.Users.Dtos.Request
{
    public class LogoutUserRequestDto : IDtoData
    {
        /// <summary>
        /// Logged in user token
        /// </summary>
        public string Token { get; set; }

        public void Sanitize(HtmlSanitizer sanitizer)
        {
        }
    }
}