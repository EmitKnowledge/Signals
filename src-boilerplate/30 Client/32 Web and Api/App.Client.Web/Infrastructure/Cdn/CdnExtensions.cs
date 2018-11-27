using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using App.Client.Web.Configuration;

namespace System.Web.Mvc
{
    /// <summary>
    /// CDN static resources helpers
    /// </summary>
    public static class CdnExtensions
    {
        private static readonly Regex ScriptRegex = new Regex("<script.+?src=[\"'](.+?)[\"'].*?>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);
        private static readonly Regex CssRegex = new Regex("<link.+?href=[\"'](.+?)[\"'].*?>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

        public static string CdnContent(this UrlHelper url, string link, bool isDynamicResource = false)
        {
            link = link.ToLower();

            // last write date ticks to hex
            string cacheBreaker = string.Empty;

            if (!isDynamicResource)
            {
                var queryStringLocation = link.IndexOf(@"?");
                if (queryStringLocation > 0)
                {
                    link = link.Substring(0, queryStringLocation);
                }
                cacheBreaker = Convert.ToString(File.GetLastWriteTimeUtc(url.RequestContext.HttpContext.Request.MapPath(link)).Ticks, 16);
            }

            link = link.TrimStart(new[] {'~', '/'});
            link = string.Format("{0}/{1}", AppWebConfiguration.Instance.StaticResourcesConfiguration.CdnUrl, link);

            // returns the file URL in static domain
            return isDynamicResource ? link : string.Format("{0}?v={1}", link, cacheBreaker);
        }

        public static MvcHtmlString CdnContent(this UrlHelper url, MvcHtmlString mvcLink, bool isDynamicResource = false, bool flatResources = false)
        {
            var context = mvcLink.ToHtmlString().ToLower();
            // match the scripts
            var linksMatches = ScriptRegex.Matches(context);
            var sb = new StringBuilder();
            foreach (Match linkMatch in linksMatches)
            {
                var link = linkMatch.Groups[1].Value;
                link = CdnContent(url, link, isDynamicResource);
                var script = string.Format("<script type='text/javascript' src='{0}'></script>", link);
                if (flatResources)
                {
                    sb.Append(script);
                }
                else
                {
                    sb.AppendLine(script);
                }
            }
            // match the styles
            linksMatches = CssRegex.Matches(context);
            foreach (Match linkMatch in linksMatches)
            {
                var link = linkMatch.Groups[1].Value;
                link = CdnContent(url, link, isDynamicResource);
                var css = string.Format("<link href='{0}' rel='stylesheet' type='text/css' />", link);
                if (flatResources)
                {
                    sb.Append(css);
                }
                else
                {
                    sb.AppendLine(css);
                }
            }

            return new MvcHtmlString(sb.ToString());
        }
    }
}