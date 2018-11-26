namespace Signals.Core.Extensions.Import.Configuration
{
    /// <summary>
    /// Represents excel importing configuration model
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    public class ExcelImportConfiguration<TResponse> : ImportConfiguration<TResponse>
    {
        /// <summary>
        /// CTOR
        /// </summary>
        public ExcelImportConfiguration()
        {
            StartingRowIndex = 1;
            SheetIndex = 0;
        }

        /// <summary>
        /// Represents zero-based index indicating the starting row for importing data
        /// </summary>
        public int StartingRowIndex { get; set; }

        /// <summary>
        /// Represents zero-based index indicating the sheet containing the import data
        /// </summary>
        public int SheetIndex { get; set; }
    }
}
