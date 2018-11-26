using OfficeOpenXml;
using Signals.Core.Common.Instance;
using Signals.Core.Extensions.ImportExport.Configuration.Export;
using Signals.Core.Extensions.ImportExport.Configuration.Import;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Signals.Core.Extensions.ImportExport.Extensions
{
    internal static class ExcelExtensions
    {
        /// <summary>
        /// Export the list of data in an excel file
        /// </summary>
        /// <returns></returns>
        public static MemoryStream Export<TData>(ExportConfiguration<TData> exportConfiguration, List<TData> data)
        {
            using (var pck = new ExcelPackage())
            {
                // Create the worksheet
                var sheet = pck.Workbook.Worksheets.Add(exportConfiguration.FileName);
                sheet.Cells.Style.Numberformat.Format = "@";

                // Execute the func
                var headers = exportConfiguration.DataMapper.Keys.ToList();

                // Set columns headers
                for (var i = 0; i < headers.Count; i++)
                {
                    sheet.Cells[1, i + 1].Value = headers[i];
                }

                // Fill the data
                if (data.Any())
                {
                    var loadedData = data.Select(x => exportConfiguration.DataMapper.Values.Select(y => y(x)).ToArray()).ToList();
                    sheet.Cells["A2"].LoadFromArrays(loadedData);
                }

                // Apply header style
                using (var rng = sheet.Cells["A1:BZ1"])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.ShrinkToFit = true;
                }

                sheet.Cells[sheet.Dimension.Address].AutoFitColumns();

                // Create the file stream
                var stream = new MemoryStream(pck.GetAsByteArray())
                {
                    Position = 0
                };

                return stream;
            }
        }

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
