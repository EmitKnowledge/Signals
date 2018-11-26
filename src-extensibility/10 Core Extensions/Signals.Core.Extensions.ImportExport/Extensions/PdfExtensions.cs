using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SelectPdf;
using PdfPageOrientation = SelectPdf.PdfPageOrientation;
using PdfPageSize = SelectPdf.PdfPageSize;
using Signals.Core.Extensions.ImportExport.Configuration.Export;

namespace Signals.Core.Extensions.ImportExport.Extensions
{
    internal static class PdfExtensions
    {
        /// <summary>
        /// Export the list of data as a PDF
        /// </summary>
        /// <returns></returns>
        public static MemoryStream Export<TData>(PdfExportConfiguration<TData> exportConfiguration, List<TData> data)
        {
            // Start building the table
            var htmlTable = new StringBuilder("<table cellspacing='0'>");

            // Execute the func
            var headers = exportConfiguration.DataMapper.Keys.ToList();

            // Render columns headers
            foreach (var header in headers)
            {
                htmlTable = htmlTable.AppendLine($"<th>{header}</th>");
            }

            // Fill the data
            if (data.Any())
            {
                var loadedData = data.Select(x => exportConfiguration.DataMapper.Values.Select(y => y(x)).ToArray()).ToList();

                foreach (var itemRow in loadedData)
                {
                    htmlTable = htmlTable.AppendLine("<tr>");
                    foreach (var item in itemRow)
                    {
                        htmlTable = htmlTable.AppendLine($"<td>{item}</td>");
                    }
                    htmlTable = htmlTable.AppendLine("</tr>");
                }
            }

            // Finish building the table
            htmlTable = htmlTable.AppendLine("</table>");

            // Render the pdf
            var html = DefaultDocument(htmlTable.ToString());
            var converter = new HtmlToPdf();
            converter.Options.PdfPageOrientation = (PdfPageOrientation)(int)exportConfiguration.PdfPageOrientation;
            converter.Options.MarginTop = exportConfiguration.MarginTop;
            converter.Options.MarginBottom = exportConfiguration.MarginBottom;
            converter.Options.MarginLeft = exportConfiguration.MarginLeft;
            converter.Options.MarginRight = exportConfiguration.MarginRight;
            converter.Options.PdfPageSize = (PdfPageSize)(int)exportConfiguration.PdfPageSize;

            var pdfDoc = converter.ConvertHtmlString(html);
            var stream = new MemoryStream();
            pdfDoc.Save(stream);
            stream.Position = 0;

            return stream;
        }

        private static string DefaultDocument(string body)
        {
            return @"<style>
						table {
							font-family:Arial, Helvetica, sans-serif;
							color:#757575;
							font-size:12px;	    
							border:#e4eaec 1px solid;
							word-wrap:break-word;
							table-layout: fixed;
							width: 100%;
						}
						table th {
							padding:21px 25px 22px 25px;
							border-top:1px solid #e2e4e9;
							border-bottom:1px solid #e2e4e9;
							text-align: left;
						}
						table th:first-child {
							padding-left:20px;
						}
						table tr {
							text-align: center;
							padding-left:20px;
						}
						table td:first-child {
							text-align: left;
							padding-left:20px;
							border-left: 0;
							font-weight: bold;
						}
						table td {
							padding:18px;
							border-top: 1px solid #ffffff;
							border-bottom:1px solid #e2e4e9;
							border-left: 1px solid #e2e4e9;
							text-align: left;
						}
						table tr.alt td {
							background: #f8fafc;
						}
						table tr:last-child td {
							border-bottom:0;
						}
					</style>
					[#Body#]".Replace("[#Body#]", body);
        }
    }
}