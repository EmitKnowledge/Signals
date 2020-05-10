using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace App.Common.Helpers.Localizaiton
{
    public static class StringExtensions
    {
        /// <summary>
        /// Transform string to slug
        /// </summary>
        /// <param name="str"></param>
        /// <param name="delimiter"></param>
        /// <param name="removeDiacritics"></param>
        /// <returns></returns>
        public static string ToSlug(this string str, string delimiter = "-", bool removeDiacritics = true)
        {
            if (string.IsNullOrEmpty(str)) return str;

            if (removeDiacritics)
            {
                str = new string(str.Normalize(NormalizationForm.FormD)
                    .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    .ToArray())
                    .Normalize(NormalizationForm.FormC);
            }

            // invalid chars           
            str = new string(str.Where(x => char.IsLetterOrDigit(x) || char.IsWhiteSpace(x)).ToArray());

            // convert multiple spaces into one space   
            str = Regex.Replace(str, @"\s+", " ").Trim();
            
            str = Regex.Replace(str.Trim(), @"\s", delimiter); // hyphens   

            return str;
        }
    }
}
