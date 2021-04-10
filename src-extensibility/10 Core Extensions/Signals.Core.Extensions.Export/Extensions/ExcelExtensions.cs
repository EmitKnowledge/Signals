using OfficeOpenXml;
using Signals.Core.Common.Instance;
using Signals.Core.Extensions.Export.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Signals.Core.Extensions.Export.Extensions
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
    }
}
