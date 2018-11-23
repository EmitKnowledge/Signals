using Ganss.XSS;

namespace Signals.Core.Processing.Input
{
    public interface IDtoData
    {
        void Sanitize(HtmlSanitizer sanitizer);
    }
}