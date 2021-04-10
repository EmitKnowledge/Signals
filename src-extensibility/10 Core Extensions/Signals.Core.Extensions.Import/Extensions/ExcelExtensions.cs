using OfficeOpenXml;
using Signals.Core.Common.Instance;
using Signals.Core.Extensions.Import.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Signals.Core.Extensions.Import.Extensions
{
    internal static class ExcelExtensions
    {
        /// <summary>
        /// Loads the data from excel file and returns it as list
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="stream"></param>
        /// <param name="importConfiguration"></param>
        /// <returns></returns>
        public static List<TResponse> Import<TResponse>(Stream stream, ExcelImportConfiguration<TResponse> importConfiguration) where TResponse : new()
        {
            var result = new List<TResponse>();
            using (var excelPackage = new ExcelPackage(stream))
            {
                var worksheet = excelPackage.Workbook.Worksheets[importConfiguration.SheetIndex];
                var headers = new List<string>();

                // Load the file's headers
                for (var i = 1; i <= worksheet.Dimension.End.Column; i++)
                {
                    var header = worksheet.Cells[1, i]?.Value?.ToString().Trim().ToLower();
                    if (header.IsNullOrEmpty()) break;
                    headers.Add(header);
                }

                // Iterate through all rows that contain data
                for (var i = importConfiguration.StartingRowIndex + 1; i <= worksheet.Dimension.End.Row; i++)
                {
                    var entry = new TResponse();
                    var hasData = false;

                    // Iterate through each map in the reader config
                    foreach (var dataHandler in importConfiguration.DataHandlers)
                    {
                        var cellValues = new List<string>();

                        // In case more than one column is needed for some property
                        foreach (var header in dataHandler.Key)
                        {
                            var headerIndex = headers.IndexOf(header.Trim().ToLower());
                            if (headerIndex == -1)
                                continue;

                            var cellValue = worksheet.Cells[i, headerIndex + 1]?.Value?.ToString().Trim();

                            if (cellValue.IsNullOrEmpty()) continue;

                            cellValues.Add(cellValue);
                            hasData = true;
                        }

                        // Send the value(s) from the cells for processing
                        dataHandler.Value(cellValues.ToArray(), entry);
                    }

                    if (!hasData && i >= importConfiguration.StartingRowIndex + 1)
                        break;

                    result.Add(entry);
                }
            }

            return result;
        }
    }
}
