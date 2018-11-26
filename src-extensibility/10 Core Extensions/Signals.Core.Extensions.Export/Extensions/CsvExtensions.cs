using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Signals.Core.Extensions.Export.Configuration;

namespace Signals.Core.Extensions.Export.Extensions
{
    internal static class CsvExtensions
    {
        public static MemoryStream Export<TData>(ExportConfiguration<TData> exportConfiguration, List<TData> data)
        {
            using (var pck = new ExcelPackage())
            {
                // Create the worksheet
                var sheet = pck.Workbook.Worksheets.Add(exportConfiguration.FileName);

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

                // Create the file stream
                var stream = new MemoryStream(EpplusCsvConverter.ConvertToCsv(pck))
                {
                    Position = 0
                };

                return stream;
            }
        }

        private static class EpplusCsvConverter
        {
            private static string DuplicateTicksForSql(string s)
            {
                return s.Replace("'", "''");
            }

            /// <summary>
            /// Takes a List collection of string and returns a delimited string.  Note that it's easy to create a huge list that won't turn into a huge string because
            /// the string needs contiguous memory.
            /// </summary>
            /// <param name="list">The input List collection of string objects</param>
            /// <param name="qualifier">
            /// The default delimiter. Using a colon in case the List of string are file names,
            /// since it is an illegal file name character on Windows machines and therefore should not be in the file name anywhere.
            /// </param>
            /// <param name="delimiter"></param>
            /// <param name="insertSpaces">Whether to insert a space after each separator</param>
            /// <param name="duplicateTicksForSql"></param>
            /// <returns>A delimited string</returns>
            /// <remarks>This was implemented pre-linq</remarks>
            private static string ToDelimitedString(List<string> list, string delimiter = ":", bool insertSpaces = false, string qualifier = "", bool duplicateTicksForSql = false)
            {
                var result = new StringBuilder();
                for (var i = 0; i < list.Count; i++)
                {
                    var initialStr = duplicateTicksForSql ? DuplicateTicksForSql(list[i]) : list[i];
                    result.Append((qualifier == string.Empty) ? initialStr : string.Format("{1}{0}{1}", initialStr, qualifier));
                    if (i < list.Count - 1)
                    {
                        result.Append(delimiter);
                        if (insertSpaces)
                        {
                            result.Append(' ');
                        }
                    }
                }
                return result.ToString();
            }

            /// <summary>
            /// Write the row to the stream
            /// </summary>
            /// <param name="record">List of cell values</param>
            /// <param name="sw">Open Writer to file</param>
            /// <param name="rowNumber">Current row num</param>
            /// <param name="totalRowCount"></param>
            /// <remarks>Avoiding writing final empty line so bulk import processes can work.</remarks>
            private static void WriteRecord(List<string> record, StreamWriter sw, int rowNumber, int totalRowCount)
            {
                var commaDelimitedRecord = ToDelimitedString(record, ",");

                if (rowNumber == totalRowCount)
                {
                    sw.Write(commaDelimitedRecord);
                }
                else
                {
                    sw.WriteLine(commaDelimitedRecord);
                }
            }

            /// <summary>
            /// Create the row to a CSV row
            /// </summary>
            /// <param name="worksheet"></param>
            /// <param name="currentRow"></param>
            /// <param name="currentRowNum"></param>
            /// <param name="maxColumnNumber"></param>
            private static void BuildRow(ExcelWorksheet worksheet, List<string> currentRow, int currentRowNum, int maxColumnNumber)
            {
                for (var i = 1; i <= maxColumnNumber; i++)
                {
                    var cell = worksheet.Cells[currentRowNum, i];
                    AddCellValue(cell == null ? string.Empty : GetCellText(cell), currentRow);
                }
            }

            /// <summary>
            /// Can't use .Text: http://epplus.codeplex.com/discussions/349696
            /// </summary>
            /// <param name="cell"></param>
            /// <returns></returns>
            private static string GetCellText(ExcelRangeBase cell)
            {
                return cell.Value?.ToString() ?? string.Empty;
            }

            /// <summary>
            /// Add the provided value
            /// </summary>
            /// <param name="s"></param>
            /// <param name="record"></param>
            private static void AddCellValue(string s, List<string> record)
            {
                record.Add(string.Format("{0}{1}{0}", '"', s));
            }

            /// <summary>
            /// Convert the package to a CSV
            /// </summary>
            /// <param name="package"></param>
            /// <returns></returns>
            public static byte[] ConvertToCsv(ExcelPackage package)
            {
                var worksheet = package.Workbook.Worksheets[0];

                var maxColumnNumber = worksheet.Dimension.End.Column;
                var currentRow = new List<string>(maxColumnNumber);
                var totalRowCount = worksheet.Dimension.End.Row;
                var currentRowNum = 1;

                var memory = new MemoryStream();

                using (var writer = new StreamWriter(memory, Encoding.ASCII))
                {
                    while (currentRowNum <= totalRowCount)
                    {
                        BuildRow(worksheet, currentRow, currentRowNum, maxColumnNumber);
                        WriteRecord(currentRow, writer, currentRowNum, totalRowCount);
                        currentRow.Clear();
                        currentRowNum++;
                    }
                }

                return memory.ToArray();
            }
        }
    }
}