using Signals.Core.Common.Instance;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Signals.Core.Common.Urls
{
    /// <summary>
    /// Url helper
    /// </summary>
    public static class UrlHelper
    {
        private static string UrlRegexPattern = @"(https?:\/\/(?:www\.|(?!www))[^\s\.]+\.[^\s]{2,}|www\.[^\s]+\.[^\s]{2,})";
        private static readonly Regex UrlRegex = new Regex(UrlRegexPattern, RegexOptions.IgnoreCase);

        /// <summary>
        /// Create Url from string
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Uri ToUrl(string url)
        {
            var isValid = IsValid(url);

            if (!isValid) return null;

            url = Fix(url);

            var _url = new Uri(url);

            if (_url.IsLoopback) return null;

            return _url;
        }

        /// <summary>
        /// Fix url prefix
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string Fix(string url)
        {
            var _url = url.TrimEnd('/').Trim();
            if (_url.StartsWith("www"))
            {
                _url = $"http://{_url}";
            }
            else if (_url.StartsWith("//"))
            {
                _url = $"http:{_url}";
            }

            var match = UrlRegex.Match(_url);
            if (match.Value.IsNullOrEmpty())
            {
                _url = $"http://{_url}";
            }

            match = UrlRegex.Match(_url);
            if (!match.IsNull())
            {
                _url = match.Value;
            }

            return _url;
        }

        /// <summary>
        /// Check if url is valid
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool IsValid(string url)
        {
            if (url.IsNullOrEmpty()) return false;
            return Uri.IsWellFormedUriString(url, UriKind.Absolute);
        }

        /// <summary>
        /// Return base url from url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetBaseFrom(this string url)
        {
            try
            {
                var uri = new Uri(url);
                var baseUri = uri.DnsSafeHost;
                return baseUri;
            }
            catch (Exception)
            {
                return url;
            }
        }

        /// <summary>
        /// Return extension from url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetExtensionFrom(this string url)
        {
            if (!IsValid(url)) return null;

            var uri = new Uri(url);

            var hasExtension = Path.HasExtension(uri.AbsolutePath);
            if (!hasExtension) return null;

            var path = $"{uri.Scheme}{Uri.SchemeDelimiter}{uri.Authority}{uri.AbsolutePath}";
            var extension = Path.GetExtension(path);
            return extension;
        }
    }
}