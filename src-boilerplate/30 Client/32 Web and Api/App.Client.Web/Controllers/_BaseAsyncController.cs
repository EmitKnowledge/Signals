using System.Text.RegularExpressions;
using System.Web.Mvc;
using App.Client.SecurityProvider.Entities;
using App.Client.SecurityProvider.Provider;
using App.Client.Web.Infrastructure.Results;
using App.Common.Helpers.Instance;
using App.Common.Helpers.Regexes;
using App.Service.ControllerInstance;

namespace App.Client.Web.Controllers
{
    public class BaseAsyncController : AsyncController
    {
        /// <summary>
        /// Represents the domain services container
        /// </summary>
        protected AppControllerService DomainServices => AppControllerService.Instance(CurrentUser);

	    /// <summary>
        /// Return the logged in user in the system
        /// </summary>
        public SecurityUser CurrentUser => MvcFormsSecurityProvider.CurrentUser;

	    /// <summary>
        /// Return the logged in user in the system
        /// </summary>
        public SecurityUser CurrentUserAsClone
        {
            get { 
                if (CurrentUser.IsNull()) return null;
                var user = new SecurityUser(CurrentUser);
                return user;
            }
        }

        /// <summary>
        /// Check if request comes from a chrome browser (mobile/desktop/tablet)
        /// </summary>
        /// <returns></returns>
        public bool IsChrome()
        {
            return Request.UserAgent.IsMatch(@"chrom(e|ium)", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Creates a <see cref="T:System.Web.Mvc.JsonResult"/> object that serializes the specified object to JavaScript Object Notation (JSON) format using the content type, content encoding, and the JSON request behavior.
        /// </summary>
        /// <returns>
        /// The result object that serializes the specified object to JSON format.
        /// </returns>
        /// <param name="data">The JavaScript object graph to serialize.</param><param name="contentType">The content type (MIME type).</param><param name="contentEncoding">The content encoding.</param><param name="behavior">The JSON request behavior </param>
        protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonDotNetResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior
            };
        }

        /// <summary>
        /// Detect if the request is mobile
        /// </summary>
        /// <returns></returns>
        protected bool IsMobile()
        {
            // detect tablet and mobile devices
            var userAgent = Request.UserAgent;
            if (userAgent == null) return false;
            if (Request.Browser.IsMobileDevice ||
                userAgent.IndexOf("Android", System.StringComparison.InvariantCultureIgnoreCase) > 0 || 
                userAgent.IndexOf("iPad", System.StringComparison.InvariantCultureIgnoreCase) > 0)
            {
                return true;
            }

            return false;
        }
    }
}
