using System.IO;

namespace Signals.Core.Processing.Input.Http.Models
{
    /// <summary>
    /// Request file stream
    /// </summary>
    public class InputFile
    {
        /// <summary>
        /// File mime type
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// File name
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Form input name
        /// </summary>
        public string FormInputName { get; set; }

        /// <summary>
        /// File stream
        /// </summary>
        public Stream File { get; set; }
    }
}
