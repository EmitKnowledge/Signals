using System.Web;
using App.Common.Helpers.Instance;

namespace App.Client.Web.Infrastructure.Request
{
    public class RequestExtractor
    {
        #region Implementation of ITracker

        /// <summary>
        /// Return tracking statistics for the current context
        /// </summary>
        /// <param name="recordLocalRequests"></param>
        /// <returns></returns>
        public RequestExtractorData GetTrackingStatistic(bool recordLocalRequests = false)
        {
            var _tracking = new RequestExtractorData();

            if (HttpContext.Current.IsNull() ||
                HttpContext.Current.Request.IsNull() ||
                (HttpContext.Current.Request.IsLocal && recordLocalRequests == false)) return _tracking;

            var _context = HttpContext.Current;

            if (!_context.User.IsNull() && !_context.User.Identity.IsNull())
            {
                _tracking.Username = _context.User.Identity.Name;
            }

            _tracking.IsAuthenticated = _context.Request.IsAuthenticated;

            if (!_context.Request.Url.IsNull())
            {
                _tracking.RawUri = _context.Request.Url.PathAndQuery;
                _tracking.BaseUri = _context.Request.Url.AbsolutePath.ToLower();
                _tracking.QueryString = _context.Request.Url.Query.IsNullOrEmpty() ? null : _context.Request.Url.Query;
            }

            if (!_context.Request.HttpMethod.IsNullOrEmpty())
            {
                _tracking.HttpMethod = _context.Request.HttpMethod.ToUpper();
            }

            _tracking.IpAddress = GetIpAddress(_context.Request);
            _tracking.Refferer = GetUrlRefferrer(_context.Request);
            _tracking.UserAgent = _context.Request.UserAgent.IsNullOrEmpty() ? null : _context.Request.UserAgent.ToLower();
            if (!_context.Request.Browser.IsNull())
            {
                _tracking.Browser = _context.Request.Browser.Browser;
                _tracking.BrowserVersion = _context.Request.Browser.Version;
                _tracking.Platform = _context.Request.Browser.Platform;
            }
            _tracking.IsCrawler = !_context.Request.Browser.IsNull() && _context.Request.Browser.Crawler;
            _tracking.IsSecureResource = _context.Request.IsSecureConnection;

            return _tracking;
        }

        /// <summary>
        /// Get ip address from request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private string GetIpAddress(HttpRequest request)
        {
            if (request.ServerVariables.IsNull()) return null;

            var _realAddress = request.ServerVariables[@"HTTP_X_FORWARDED_FOR"];
            if (_realAddress.IsNullOrEmpty())
            {
                _realAddress = request.ServerVariables[@"HTTP_FORWARDED"];
            }
            if (_realAddress.IsNullOrEmpty())
            {
                _realAddress = request.ServerVariables[@"REMOTE_ADDR"];
            }

            return _realAddress;
        }

        /// <summary>
        /// Return refferer from request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private string GetUrlRefferrer(HttpRequest request)
        {
            if (request.UrlReferrer.IsNull()) return null;
            return request.UrlReferrer.AbsoluteUri;
        }

        #endregion
    }
}