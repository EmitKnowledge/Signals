using System;
using System.Net;
using System.Text;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace App.Client.Web.Infrastructure.Results
{
    /// <summary>
    /// Custom ObjectId serialization
    /// </summary>
    public class JsonDotNetResult : JsonResult
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            //Converters = new List<JsonConverter> { new ObjectIdJsonConvertor() }
        };

        /// <summary>
        /// Enables processing of the result of an action method by a custom type that inherits from the <see cref="T:System.Web.Mvc.ActionResult"/> class.
        /// </summary>
        /// <param name="context">The context within which the result is executed.</param><exception cref="T:System.ArgumentNullException">The <paramref name="context"/> parameter is null.</exception>
        public override void ExecuteResult(ControllerContext context)
        {
            var response = context.HttpContext.Response;
            response.ContentType = !string.IsNullOrEmpty(this.ContentType) ? this.ContentType : "application/json";

            if (this.JsonRequestBehavior == JsonRequestBehavior.DenyGet &&
                string.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
            {
                response.Clear();
                response.ContentEncoding = Encoding.UTF8;
                response.HeaderEncoding = Encoding.UTF8;
                response.TrySkipIisCustomErrors = true;
                response.StatusCode = (int)HttpStatusCode.Forbidden;
                return;
            }

            if (this.ContentEncoding != null)
            {
                response.ContentEncoding = this.ContentEncoding;
            }

            if (this.Data == null)
            {
                return;
            }

            response.Write(JsonConvert.SerializeObject(this.Data, Settings));
        }
    }
}