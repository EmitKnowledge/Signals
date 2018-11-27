using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace App.Client.Web.Infrastructure.Css
{
    public static class CssHelper
    {
        public static IHtmlString EmbedCss(this HtmlHelper htmlHelper, params string[] path)
        {
            var sb = new StringBuilder();
            sb.AppendLine(@"<style>");

            foreach (var s in path)
            {
                // take a path that starts with "~" and map it to the filesystem.
                var cssFilePath = HttpContext.Current.Server.MapPath(s);
                // load the contents of that file
                try
                {
                    var cssText = File.ReadAllText(cssFilePath);
                    sb.AppendLine(cssText);
                }
                catch
                {
                    // return nothing if we can't read the file for any reason
                }
            }

            sb.AppendLine(@"</style>");
            return htmlHelper.Raw(sb);
        }
    }
}