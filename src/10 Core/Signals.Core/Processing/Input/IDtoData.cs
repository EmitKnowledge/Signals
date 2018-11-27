using Ganss.XSS;

namespace Signals.Core.Processing.Input
{
    /// <summary>
    /// Dto data constraint
    /// </summary>
    public interface IDtoData
    {
        /// <summary>
        /// Sanitize the HTML data
        /// </summary>
        /// <param name="sanitizer"></param>
        void Sanitize(HtmlSanitizer sanitizer);
    }
}