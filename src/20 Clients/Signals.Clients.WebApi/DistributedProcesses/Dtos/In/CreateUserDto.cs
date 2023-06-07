using Ganss.Xss;
using Signals.Core.Processing.Input;

namespace Signals.Clients.WebApi.DistributedProcesses.Dtos.In
{
    public class CreateUserDto : DtoData<CreateUserDto>, IDtoData
    {
        public string Email { get; set; }

        public void Sanitize(HtmlSanitizer sanitizer)
        {
            Email = sanitizer.Sanitize(Email);
        }

        public override CreateUserDto Map()
        {
            return this;
        }
    }
}
