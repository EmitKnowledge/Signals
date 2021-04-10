using Signals.Core.Common.Instance;
using System.IO;

namespace Signals.Core.Common.InputOutput
{
    /// <summary>
    /// Memory stream with a name
    /// </summary>
    public class NamedMemoryStream : MemoryStream
    {
        /// <summary>
        /// Name of the stream
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Create named memory strem from the provided stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static NamedMemoryStream From(Stream stream, string name)
        {
            if (stream.IsNull()) return null;
            var ms = new NamedMemoryStream();
            ms.Name = name ?? @"";
            stream.CopyTo(ms);
            ms.Position = 0;
            return ms;
        }
    }
}