using HtmlAgilityPack;
using Signals.Core.Common.Instance;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Signals.Core.Common.Text
{
    /// <summary>
    /// Html to text convertor
    /// </summary>
    public static class HtmlExtensions
    {
        /// <summary>
        /// Convert html to text
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string ConvertHtml(string html)
        {
            if (html.IsNullOrEmpty()) return html;
            try
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                var sw = new StringWriter();
                ConvertTo(doc.DocumentNode, sw);
                sw.Flush();
                var result = sw.ToString();
                return result.Trim();
            }
            catch (Exception)
            {
                return html;
            }
        }

        /// <summary>
        /// Convert html to text
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string ConvertToText(string html)
        {
            if (html.IsNullOrEmpty()) return html;
            try
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(html);
                var pageText = HtmlEntity.DeEntitize(doc.DocumentNode.InnerText);
                return pageText;
            }
            catch (Exception)
            {
                return html;
            }
        }

        private static void ConvertContentTo(HtmlNode node, TextWriter outText)
        {
            foreach (var subnode in node.ChildNodes)
            {
                ConvertTo(subnode, outText);
            }
        }

        private static void ConvertTo(HtmlNode node, TextWriter outText)
        {
            string html;
            switch (node.NodeType)
            {
                case HtmlNodeType.Comment:
                    // don't output comments
                    break;

                case HtmlNodeType.Document:
                    ConvertContentTo(node, outText);
                    break;

                case HtmlNodeType.Text:
                    // script and style must not be output
                    string parentName = node.ParentNode.Name;
                    if ((parentName == "script") || (parentName == "style"))
                        break;

                    // get text
                    html = ((HtmlTextNode)node).Text;

                    // is it in fact a special closing node output as text?
                    if (HtmlNode.IsOverlappedClosingElement(html))
                        break;

                    // check the text is meaningful and not a bunch of whitespaces
                    if (html.Trim().Length > 0)
                    {
                        outText.Write(HtmlEntity.DeEntitize(html));
                    }
                    break;

                case HtmlNodeType.Element:
                    switch (node.Name)
                    {
                        case "a":
                            if (!node.HasChildNodes)
                            {
                                outText.Write(node.InnerText);
                            }
                            break;

                        case "p":
                            // treat paragraphs as crlf
                            outText.Write("\r\n");
                            break;

                        case "br":
                            // treat paragraphs as crlf
                            outText.Write("\r\n");
                            break;
                    }

                    if (node.HasChildNodes)
                    {
                        ConvertContentTo(node, outText);
                    }
                    break;
            }
        }

        /// <summary>
        /// Return a list of image src from html
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static List<string> GetImages(string html)
        {
            if (string.IsNullOrEmpty(html)) return new List<string>();
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            List<HtmlNode> imgsNodes = (from x in doc.DocumentNode.Descendants()
                                        where x.Name.ToLower() == "img"
                                        select x).ToList();
            var imgSrc = new List<string>();
            for (int i = 0; i < imgsNodes.Count(); i++)
            {
                var imgNode = imgsNodes[i];
                if (imgNode.Attributes.Contains(@"src") &&
                   !string.IsNullOrEmpty(imgNode.Attributes["src"].Value))
                {
                    imgSrc.Add(imgNode.Attributes["src"].Value);
                }
                else if (imgNode.Attributes.Contains(@"srcset") &&
                         !string.IsNullOrEmpty(imgNode.Attributes["srcset"].Value))
                {
                    var srcSet = imgNode.Attributes["srcset"].Value.Split(',').LastOrDefault();
                    if (!srcSet.IsNullOrEmpty())
                    {
                        imgSrc.Add(srcSet);
                    }
                }
            }

            return imgSrc;
        }

        /// <summary>
        /// Create links from text
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public static string Linkify(string searchText)
        {
            var regx = new Regex(@"\b(((\S+)?)(@|mailto\:|(news|(ht|f)tp(s?))\://)\S+)\b", RegexOptions.IgnoreCase);
            searchText = searchText.Replace("&nbsp;", " ");
            var matches = regx.Matches(searchText);

            foreach (Match match in matches)
            {
                if (match.Value.StartsWith("http"))
                { // if it starts with anything else then dont linkify -- may already be linked!
                    searchText = searchText.Replace(match.Value, "<a href='" + match.Value + "' target='_blank'>" + match.Value + "</a>");
                }
            }

            return searchText;
        }

        /// <summary>
        /// Correct new line DB new lines to new lines for web components
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ConvertToHtmlNewLines(this string text)
        {
            if (text.IsNullOrEmpty()) { return text; }
            var celaredText = text.Replace("<br>", "\r\n");
            return celaredText;
        }

        /// <summary>
        /// Correct new line DB new lines to new lines for web components
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ConvertNewLinesToHtml(this string text)
        {
            if (text.IsNullOrEmpty()) { return text; }
            var celaredText = text.Replace("\r\n", "</br>");
            return celaredText;
        }
    }
}