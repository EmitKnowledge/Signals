using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace Signals.Core.Processing.Results
{
    /// <summary>
    /// Export file result
    /// </summary>
    [Serializable]
    [DataContract]
    public class FileResult : MethodResult<Stream>
    {
        /// <summary>
        /// Exported file name
        /// </summary>
        [DataMember]
        public string FileName { get; set; }

        /// <summary>
        /// Exported file mime type
        /// </summary>
        [DataMember]
        public string MimeType { get; set; }

        /// <summary>
        /// CTOR
        /// </summary>
        [DebuggerStepThrough]
        public FileResult() { }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fileName"></param>
        /// <param name="mime"></param>
        public FileResult(Stream file, string fileName, string mimeType)
        {
            Result = file;
            FileName = fileName;
            MimeType = mimeType;
        }
    }
}
