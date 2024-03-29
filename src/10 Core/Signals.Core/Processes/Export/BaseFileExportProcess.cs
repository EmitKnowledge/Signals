﻿using System.Collections.Generic;
using Signals.Aspects.DI.Attributes;
using Signals.Core.Processes.Base;
using Signals.Core.Processes.Import;
using Signals.Core.Processing.Input;
using Signals.Core.Processing.Results;

namespace Signals.Core.Processes.Export
{
    /// <summary>
    /// Represents a file export process
    /// </summary>
    internal interface IFileExportProcess { }

    /// <summary>
    /// Represents a file export process
    /// </summary>
    public abstract class BaseFileExportProcess<TData> : BaseProcess<FileResult>,
        IFileExportProcess
    {
        /// <summary>
        /// File export process context
        /// </summary>
        [Import]
        protected  virtual IFileExportProcessContext Context
        {
            get => _context;
            set { (value as FileExportProcessContext)?.SetProcess(this); _context = value; }
        }
        private IFileExportProcessContext _context;

        /// <summary>
        /// Base process context override
        /// </summary>
        internal override IBaseProcessContext BaseContext => Context;

        /// <summary>
        /// Represents the export configuration model
        /// </summary>
        protected abstract IExportConfiguration<TData> ExportConfiguration { get; }

        /// <summary>
        /// Returns implementation of <see cref="IFileImporter{TData}"/>
        /// </summary>
        /// <returns></returns>
        protected abstract IFileExporter<TData> ResolveFileExporter();

        /// <summary>
        /// Authentication and authorization step
        /// </summary>
        /// <returns></returns>
        public abstract FileResult Auth();

        /// <summary>
        /// Validation step
        /// </summary>
        /// <returns></returns>
        public abstract FileResult Validate();

        /// <summary>
        /// Returns list of type TData for exporting
        /// </summary>
        /// <returns></returns>
        protected abstract List<TData> DataSource();
        
        /// <summary>
        /// Entry execution point
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        internal override FileResult ExecuteProcess(params object[] args)
        {
            return Execute();
        }

        /// <summary>
        /// Execution using base strategy
        /// </summary>
        /// <returns></returns>
        internal FileResult Execute()
        {
            var result = Auth();
            this.D("Executed -> Auth.");
            if (result.IsFaulted)
            {
	            this.D("Executed -> Auth -> Failed.");
                return result;
            }

            result = Validate();
            this.D("Executed -> Validate.");
            if (result.IsFaulted)
            {
	            this.D("Executed -> Validation -> Failed.");
                return result;
            }

            // Map data to memory stream
            var fileExporter = ResolveFileExporter();
            this.D($"File exporter of type {fileExporter.MimeType} resolved.");

            var stream = fileExporter.Export(ExportConfiguration, DataSource());
            this.D($"File of type: {fileExporter.MimeType} exported to stream.");

            return new FileResult(stream, ExportConfiguration.FileName, fileExporter.MimeType);
        }
    }

    /// <summary>
    /// Represents a file export process
    /// </summary>
    public abstract class BaseFileExportProcess<TData, T1> : BaseProcess<FileResult>,
        IFileExportProcess
        where T1 : IDtoData
    {
        /// <summary>
        /// File export process context
        /// </summary>
        [Import]
        protected virtual IFileExportProcessContext Context
        {
            get => _context;
            set { (value as FileExportProcessContext)?.SetProcess(this); _context = value; }
        }
        private IFileExportProcessContext _context;

        /// <summary>
        /// Base process context override
        /// </summary>
        internal override IBaseProcessContext BaseContext => Context;

        /// <summary>
        /// Represents the export configuration model
        /// </summary>
        protected abstract IExportConfiguration<TData> ExportConfiguration { get; }

        /// <summary>
        /// Returns implementation of <see cref="IFileImporter{TData}"/>
        /// </summary>
        /// <returns></returns>
        protected abstract IFileExporter<TData> ResolveFileExporter();

        /// <summary>
        /// Authentication and authorization step
        /// </summary>
        /// <returns></returns>
        public abstract FileResult Auth(T1 obj);

        /// <summary>
        /// Validation step
        /// </summary>
        /// <returns></returns>
        public abstract FileResult Validate(T1 obj);

        /// <summary>
        /// Returns list of type TData for exporting
        /// </summary>
        /// <returns></returns>
        protected abstract List<TData> DataSource(T1 obj);
        
        /// <summary>
        /// Entry execution point
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        internal override FileResult ExecuteProcess(params object[] args)
        {
            var obj = (T1)args[0];
            return Execute(obj);
        }

        /// <summary>
        /// Execution using base strategy
        /// </summary>
        /// <returns></returns>
        internal FileResult Execute(T1 obj)
        {
            var result = Auth(obj);
            if (result.IsFaulted)
            {
	            return result;
            }

            result = Validate(obj);
            if (result.IsFaulted)
            {
	            return result;
            }

            // Map data to memory stream
            var fileExporter = ResolveFileExporter();
            this.D($"File exporter of type {fileExporter.MimeType} resolved.");

            var stream = fileExporter.Export(ExportConfiguration, DataSource(obj));
            this.D($"File of type: {fileExporter.MimeType} exported to stream.");

            return new FileResult(stream, ExportConfiguration.FileName, fileExporter.MimeType);
        }
    }

    /// <summary>
    /// Represents a file export process
    /// </summary>
    public abstract class BaseFileExportProcess<TData, T1, T2> : BaseProcess<FileResult>,
        IFileExportProcess
        where T1 : IDtoData
        where T2 : IDtoData
    {
        /// <summary>
        /// File export process context
        /// </summary>
        [Import]
        protected virtual IFileExportProcessContext Context
        {
            get => _context;
            set { (value as FileExportProcessContext)?.SetProcess(this); _context = value; }
        }
        private IFileExportProcessContext _context;

        /// <summary>
        /// Base process context override
        /// </summary>
        internal override IBaseProcessContext BaseContext => Context;

        /// <summary>
        /// Represents the export configuration model
        /// </summary>
        protected abstract IExportConfiguration<TData> ExportConfiguration { get; }

        /// <summary>
        /// Returns implementation of <see cref="IFileImporter{TData}"/>
        /// </summary>
        /// <returns></returns>
        protected abstract IFileExporter<TData> ResolveFileExporter();

        /// <summary>
        /// Authentication and authorization step
        /// </summary>
        /// <returns></returns>
        public abstract FileResult Auth(T1 obj1, T2 obj2);

        /// <summary>
        /// Validation step
        /// </summary>
        /// <returns></returns>
        public abstract FileResult Validate(T1 obj1, T2 obj2);

        /// <summary>
        /// Returns list of type TData for exporting
        /// </summary>
        /// <returns></returns>
        protected abstract List<TData> DataSource(T1 obj1, T2 obj2);
        
        /// <summary>
        /// Entry execution point
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        internal override FileResult ExecuteProcess(params object[] args)
        {
            var obj1 = (T1)args[0];
            var obj2 = (T2)args[1];
            return Execute(obj1, obj2);
        }

        /// <summary>
        /// Execution using base strategy
        /// </summary>
        /// <returns></returns>
        internal FileResult Execute(T1 obj1, T2 obj2)
        {
            var result = Auth(obj1, obj2);
            if (result.IsFaulted)
            {
	            return result;
            }

            result = Validate(obj1, obj2);
            if (result.IsFaulted)
            {
	            return result;
            }

            // Map data to memory stream
            var fileExporter = ResolveFileExporter();
            this.D($"File exporter of type {fileExporter.MimeType} resolved.");

            var stream = fileExporter.Export(ExportConfiguration, DataSource(obj1, obj2));
            this.D($"File of type: {fileExporter.MimeType} exported to stream.");

            return new FileResult(stream, ExportConfiguration.FileName, fileExporter.MimeType);
        }
    }

    /// <summary>
    /// Represents a file export process
    /// </summary>
    public abstract class BaseFileExportProcess<TData, T1, T2, T3> : BaseProcess<FileResult>,
        IFileExportProcess
        where T1 : IDtoData
        where T2 : IDtoData
        where T3 : IDtoData
    {
        /// <summary>
        /// File export process context
        /// </summary>
        [Import]
        protected virtual IFileExportProcessContext Context
        {
            get => _context;
            set { (value as FileExportProcessContext)?.SetProcess(this); _context = value; }
        }
        private IFileExportProcessContext _context;

        /// <summary>
        /// Base process context override
        /// </summary>
        internal override IBaseProcessContext BaseContext => Context;

        /// <summary>
        /// Represents the export configuration model
        /// </summary>
        protected abstract IExportConfiguration<TData> ExportConfiguration { get; }

        /// <summary>
        /// Returns implementation of <see cref="IFileImporter{TData}"/>
        /// </summary>
        /// <returns></returns>
        protected abstract IFileExporter<TData> ResolveFileExporter();

        /// <summary>
        /// Authentication and authorization step
        /// </summary>
        /// <returns></returns>
        public abstract FileResult Auth(T1 obj1, T2 obj2, T3 obj3);

        /// <summary>
        /// Validation step
        /// </summary>
        /// <returns></returns>
        public abstract FileResult Validate(T1 obj1, T2 obj2, T3 obj3);

        /// <summary>
        /// Returns list of type TData for exporting
        /// </summary>
        /// <returns></returns>
        protected abstract List<TData> DataSource(T1 obj1, T2 obj2, T3 obj3);
        
        /// <summary>
        /// Entry execution point
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        internal override FileResult ExecuteProcess(params object[] args)
        {
            var obj1 = (T1)args[0];
            var obj2 = (T2)args[1];
            var obj3 = (T3)args[2];
            return Execute(obj1, obj2, obj3);
        }

        /// <summary>
        /// Execution using base strategy
        /// </summary>
        /// <returns></returns>
        internal FileResult Execute(T1 obj1, T2 obj2, T3 obj3)
        {
            var result = Auth(obj1, obj2, obj3);
            if (result.IsFaulted)
            {
	            return result;
            }

            result = Validate(obj1, obj2, obj3);
            if (result.IsFaulted)
            {
	            return result;
            }

            // Map data to memory stream
            var fileExporter = ResolveFileExporter();
            this.D($"File exporter of type {fileExporter.MimeType} resolved.");

            var stream = fileExporter.Export(ExportConfiguration, DataSource(obj1, obj2, obj3));
            this.D($"File of type: {fileExporter.MimeType} exported to stream.");

            return new FileResult(stream, ExportConfiguration.FileName, fileExporter.MimeType);
        }
    }
}
