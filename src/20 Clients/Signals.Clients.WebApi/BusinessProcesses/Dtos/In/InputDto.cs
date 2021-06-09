using Ganss.XSS;
using Signals.Core.Processing.Input;

namespace Signals.Clients.WebApi.BusinessProcesses.Dtos.In
{
    public class InputDto : IDtoData
    {
        public string Data { get; set; }

        public void Sanitize(HtmlSanitizer sanitizer)
        {
            
        }
    }
}
