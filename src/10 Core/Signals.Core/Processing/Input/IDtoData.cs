using Ganss.Xss;

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

    /// <summary>
    /// Dto data constraint
    /// </summary>
    public abstract class DtoData<T>
    {
        /// <summary>
        /// Maps the current instance to an object of type T
        /// </summary>
        /// <returns></returns>
        public abstract T Map();
    }
}