using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using App.Common.Base.Results;
using App.Service.Controllers._Errors;

namespace App.Client.Web.Infrastructure.Response
{
    /// <summary>
    /// Wrap sending response back to client for readability
    /// </summary>
    public static class ResponseDataEx
    {
        /// <summary>
        /// Return true
        /// </summary>
        /// <returns></returns>
        public static bool Ok()
        {
            return true;
        }

        /// <summary>
        /// Return the provided data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T Ok<T>(T data)
        {
            return data;
        }

        /// <summary>
        /// Return the provided data and user visible message
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="userVisibleMessage"></param>
        /// <returns></returns>
        public static object Ok<T>(T data, string userVisibleMessage)
        {
            return new { data, message = userVisibleMessage};
        }
    }

    public class FaultedActionResult<T> : IHttpActionResult where T : VoidResult, new()
    {
        private readonly T _model;

        private HttpStatusCode StatusCode;

        public FaultedActionResult(T model, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            _model = model;
            StatusCode = statusCode;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(StatusCode);
            response.Content = new ObjectContent<T>(_model, new JsonMediaTypeFormatter { Indent = false }, new MediaTypeHeaderValue("application/json"));
            return Task.FromResult(response);
        }
    }

    public static class ActionResponseHelper
    {
        /// <summary>
        /// if the bl processing reuslt is faulted throws 
        /// action excution exception and return result in json format
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="controller"></param>
        /// <param name="result"></param>
        public static JsonResult CreateFaultedResult<T>(this Controller controller, T result) where T : VoidResult, new()
        {
            if (result == null || !result.IsFaulted) return null;

            controller.HttpContext.Response.Clear();
            controller.HttpContext.Response.ContentEncoding = Encoding.UTF8;
            controller.HttpContext.Response.HeaderEncoding = Encoding.UTF8;
            controller.HttpContext.Response.TrySkipIisCustomErrors = true;
            controller.HttpContext.Response.StatusCode = 400;
            controller.HttpContext.Response.ContentType = "application/json; charset=utf-8";

            var _jsonResult = new JsonResult();
            _jsonResult.ContentEncoding = Encoding.UTF8;
            _jsonResult.ContentType = "application/json; charset=utf-8";
            _jsonResult.Data = result.ToOfBoundariesResult();
            _jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            return _jsonResult;
        }

        /// <summary>
        /// if the bl processing reuslt is faulted throws 
        /// action excution exception and return result in json format
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="statusCode"></param>
        /// <param name="data"></param>
        public static JsonResult CreateFaultedResult(this Controller controller, HttpStatusCode statusCode = HttpStatusCode.BadRequest, string data = null)
        {
            controller.HttpContext.Response.Clear();
            controller.HttpContext.Response.ContentEncoding = Encoding.UTF8;
            controller.HttpContext.Response.HeaderEncoding = Encoding.UTF8;
            controller.HttpContext.Response.TrySkipIisCustomErrors = true;
            controller.HttpContext.Response.StatusCode = (int)statusCode;
            controller.HttpContext.Response.ContentType = "application/json; charset=utf-8";

            var result = VoidResult.FaultedResult<MethodResult<string>>();
            result.Result = data;

            var _jsonResult = new JsonResult();
            _jsonResult.ContentEncoding = Encoding.UTF8;
            _jsonResult.ContentType = "application/json; charset=utf-8";
            _jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            _jsonResult.Data = result.ToOfBoundariesResult<MethodResult<string>, string>();;

            return _jsonResult;
        }

        /// <summary>
        /// if the bl processing result is faulted throws 
        /// action execution exception and return result in json format
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="controller"></param>
        /// <param name="data"></param>
        public static JsonResult CreateFaultedResultWithPayload<T>(this Controller controller, T data)
        {
            controller.HttpContext.Response.Clear();
            controller.HttpContext.Response.ContentEncoding = Encoding.UTF8;
            controller.HttpContext.Response.HeaderEncoding = Encoding.UTF8;
            controller.HttpContext.Response.TrySkipIisCustomErrors = true;
            controller.HttpContext.Response.StatusCode = 400;
            controller.HttpContext.Response.ContentType = "application/json; charset=utf-8";

            var result = VoidResult.FaultedResult<MethodResult<T>>();
            result.Result = data;

            var _jsonResult = new JsonResult();
            _jsonResult.ContentEncoding = Encoding.UTF8;
            _jsonResult.ContentType = "application/json; charset=utf-8";
            _jsonResult.Data = result.ToOfBoundariesResult<MethodResult<T>, T>();
            _jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            return _jsonResult;
        }
    }

    public static class ActionResponseApiHelper
    {
        /// <summary>
        /// if the bl processing reuslt is faulted throws 
        /// action excution exception and return result in json format
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="controller"></param>
        /// <param name="result"></param>
        public static IHttpActionResult CreateFaultedResult<T>(this ApiController controller, T result) where T : VoidResult, new()
        {
			if (result == null || !result.IsFaulted) return new FaultedActionResult<VoidResult>(result);
			var badRequestResult = new FaultedActionResult<VoidResult>(result.ToOfBoundariesResult());
            return badRequestResult;
        }

        /// <summary>
        /// if the bl processing reuslt is faulted throws 
        /// action excution exception and return result in json format
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="statusCode"></param>
        /// <param name="data"></param>
        /// <param name="userVisibleErrorMessage"></param>
        public static IHttpActionResult CreateFaultedResultWithPayload(this ApiController controller, HttpStatusCode statusCode = HttpStatusCode.BadRequest, string data = null, string userVisibleErrorMessage = null)
        {
            var result = VoidResult.FaultedResult<MethodResult<string>>();
            var badRequestResult = new FaultedActionResult<MethodResult<string>>(result.ToOfBoundariesResult(data, userVisibleErrorMessage), statusCode);
            return badRequestResult;
        }

        /// <summary>
        /// if the bl processing result is faulted throws 
        /// action execution exception and return result in json format
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="controller"></param>
        /// <param name="data"></param>
        /// <param name="userVisibleErrorMessage"></param>
        public static IHttpActionResult CreateFaultedResultWithPayload<T>(this ApiController controller, HttpStatusCode statusCode = HttpStatusCode.BadRequest, T data = default(T), string userVisibleErrorMessage = null)
        {
            var result = VoidResult.FaultedResult<MethodResult<T>>();
            var badRequestResult = new FaultedActionResult<MethodResult<T>>(result.ToOfBoundariesResult(data, userVisibleErrorMessage));
            return badRequestResult;
        }
    }
}