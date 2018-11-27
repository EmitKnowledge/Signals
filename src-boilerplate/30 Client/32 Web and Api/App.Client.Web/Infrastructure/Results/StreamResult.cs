using System.IO;
using System.Web.Mvc;

namespace App.Client.Web.Infrastructure.Results
{
    public class StreamResult : ViewResult
    {
        public Stream Stream { get; set; }
        public string ContentType { get; set; }
        public string ETag { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.ContentType = ContentType;
            
            if (ETag != null) context.HttpContext.Response.AddHeader("ETag", ETag);
            
            const int size = 4096;
            byte[] bytes = new byte[size];
            int numBytes;
            
            while ((numBytes = Stream.Read(bytes, 0, size)) > 0)
            {
                context.HttpContext.Response.OutputStream.Write(bytes, 0, numBytes);
            }
        }
    }
}