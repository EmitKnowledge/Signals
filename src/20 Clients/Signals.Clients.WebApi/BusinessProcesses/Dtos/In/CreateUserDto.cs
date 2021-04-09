using Ganss.XSS;
using Signals.Core.Processing.Input;

namespace Signals.Clients.WebApi.BusinessProcesses.Dtos.In
{
    public class CreateUserDto : IDtoData
    {
        public string Email { get; set; }

        public void Sanitize(HtmlSanitizer sanitizer)
        {
            Email = sanitizer.Sanitize(Email);
        }
    }
}
