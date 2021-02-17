using Ganss.XSS;
using Signals.Core.Processing.Input;

namespace App.Domain.Processes.Users.Dtos.Request
{
    public class DeleteUserRequestDto : IDtoData
    {
        /// <summary>
        /// Id of the user
        /// </summary>
        public int Id { get; set; }

        public void Sanitize(HtmlSanitizer sanitizer)
        {
        }
    }
}