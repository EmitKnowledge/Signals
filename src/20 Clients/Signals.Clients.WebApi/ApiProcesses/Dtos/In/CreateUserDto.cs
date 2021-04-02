using Ganss.XSS;
using Signals.Core.Processing.Input;

namespace Signals.Clients.WebApi.ApiProcesses.Dtos.In
{
    public class CreateUserDto : DtoData<BusinessProcesses.Dtos.In.CreateUserDto>, IDtoData
    {
        public string Email { get; set; }

        public override BusinessProcesses.Dtos.In.CreateUserDto Map()
        {
            return new BusinessProcesses.Dtos.In.CreateUserDto
            {
                Email = Email
            };
        }

        public void Sanitize(HtmlSanitizer sanitizer)
        {
            Email = sanitizer.Sanitize(Email);
        }
    }
}
