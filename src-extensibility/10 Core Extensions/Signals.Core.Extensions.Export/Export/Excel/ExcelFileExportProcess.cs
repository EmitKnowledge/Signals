﻿using Signals.Core.Processes.Export;
using Signals.Core.Processing.Input;

namespace Signals.Core.Extensions.Export.Export.Excel
{
    /// <summary>
    /// Represents process for exporting data in excel format
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public abstract class ExcelFileExportProcess<TData> : BaseFileExportProcess<TData>
    {
        /// <summary>
        /// Defines file exporter that will handle the exporting
        /// </summary>
        /// <returns></returns>
        protected override IFileExporter<TData> ResolveFileExporter()
        {
            return new ExcelFileExporter<TData>();
        }
    }

    /// <summary>
    /// Represents process for exporting data in excel format
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="T1"></typeparam>
    public abstract class ExcelFileExportProcess<TData, T1> : BaseFileExportProcess<TData, T1>
        where T1 : IDtoData
    {
        /// <summary>
        /// Defines file explorer that will handle the exporting
        /// </summary>
        /// <returns></returns>
        protected override IFileExporter<TData> ResolveFileExporter()
        {
            return new ExcelFileExporter<TData>();
        }
    }

    /// <summary>
    /// Represents process for exporting data in excel format
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public abstract class ExcelFileExportProcess<TData, T1, T2> : BaseFileExportProcess<TData, T1, T2>
        where T1 : IDtoData
        where T2 : IDtoData
    {
        /// <summary>
        /// Defines file explorer that will handle the exporting
        /// </summary>
        /// <returns></returns>
        protected override IFileExporter<TData> ResolveFileExporter()
        {
            return new ExcelFileExporter<TData>();
        }
    }

    /// <summary>
    /// Represents process for exporting data in excel format
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    public abstract class ExcelFileExportProcess<TData, T1, T2, T3> : BaseFileExportProcess<TData, T1, T2, T3>
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
            return new ExcelFileExporter<TData>();
        }
    }
}
