using Signals.Core.Business.Export;
using Signals.Core.Processing.Input;

namespace Signals.Core.Extensions.ImportExport.Export.Csv
{
    /// <summary>
    /// Represents process for exporting data of type <see cref="TData"/> in csv format
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public abstract class CsvFileExportProcess<TData> : BaseFileExportProcess<TData>
    {
        /// <summary>
        /// Defines file explorer that will handle the exporting
        /// </summary>
        /// <returns></returns>
        protected override IFileExporter<TData> ResolveFileExporter()
        {
            return new CsvFileExporter<TData>();
        }
    }

    /// <summary>
    /// Represents process for exporting data of type <see cref="TData"/> in csv format
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="T1"></typeparam>
    public abstract class CsvFileExportProcess<TData, T1> : BaseFileExportProcess<TData, T1>
        where T1 : IDtoData
    {
        /// <summary>
        /// Defines file explorer that will handle the exporting
        /// </summary>
        /// <returns></returns>
        protected override IFileExporter<TData> ResolveFileExporter()
        {
            return new CsvFileExporter<TData>();
        }
    }

    /// <summary>
    /// Represents process for exporting data of type <see cref="TData"/> in csv format
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public abstract class CsvFileExportProcess<TData, T1, T2> : BaseFileExportProcess<TData, T1, T2>
        where T1 : IDtoData
        where T2 : IDtoData
    {
        /// <summary>
        /// Defines file explorer that will handle the exporting
        /// </summary>
        /// <returns></returns>
        protected override IFileExporter<TData> ResolveFileExporter()
        {
            return new CsvFileExporter<TData>();
        }
    }

    /// <summary>
    /// Represents process for exporting data of type <see cref="TData"/> in csv format
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    public abstract class CsvFileExportProcess<TData, T1, T2, T3> : BaseFileExportProcess<TData, T1, T2, T3>
        where T1 : IDtoData
        where T2 : IDtoData
        where T3 : IDtoData
    {
        /// <summary>
        /// Defines file explorer that will handle the exporting
        /// </summary>
        /// <returns></returns>
        protected override IFileExporter<TData> ResolveFileExporter()
        {
            return new CsvFileExporter<TData>();
        }
    }
}
