using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App.Common.Helpers.Strings
{
    public static class UrlExtensions
    {
        /// <summary>
        /// Combine url root with relative path
        /// </summary>
        /// <param name="rootUrl"></param>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        public static string Combine(params string[] url)
        {
            if (!url.Any()) return null;

            return string.Join("/", url.Select(x => x.Trim('/')));
        }
    }
}
