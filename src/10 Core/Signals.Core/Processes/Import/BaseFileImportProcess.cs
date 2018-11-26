using System.IO;
using Signals.Core.Processes.Base;
using Signals.Core.Processes.Export;
using Signals.Core.Processing.Results;

namespace Signals.Core.Processes.Import
{
    /// <summary>
    /// Represents a file import process
    /// </summary>

    internal interface IFileImportProcess { }

    /// <summary>
    /// Represents a file import process
    /// </summary>
    public abstract class BaseFileImportProcess<TInput, TResponse> : BaseProcess<ListResult<TResponse>>, IFileExportProcess
        where TInput : Stream
    {
        /// <summary>
        /// File import process context
        /// </summary>
        protected FileImportProcessContext Context { get; set; }

        /// <summary>
        /// Base process context override
        /// </summary>
        internal override BaseProcessContext BaseContext => Context;

        /// <summary>
        /// Represents the import configuration model
        /// </summary>
        protected abstract IImportConfiguration<TResponse> ImportConfiguration { get; }

        /// <summary>
        /// Returns implementation of <see cref="IFileImporter{TData}"/>
        /// </summary>
        /// <returns></returns>
        protected abstract IFileImporter<TResponse> ResolveFileImporter();

        /// <summary>
        /// Authentication step
        /// </summary>
        /// <returns></returns>
        public abstract ListResult<TResponse> Authenticate(TInput stream);

        /// <summary>
        /// Authorization step
        /// </summary>
        /// <returns></returns>
        public abstract ListResult<TResponse> Authorize(TInput stream);

        /// <summary>
        /// Validation step
        /// </summary>
        /// <returns></returns>
        public abstract ListResult<TResponse> Validate(TInput stream);

        /// <summary>
        /// CTOR
        /// </summary>
        protected BaseFileImportProcess()
        {
            Context = new FileImportProcessContext();
        }

        /// <summary>
        /// Entry execution point
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        internal override ListResult<TResponse> ExecuteProcess(params object[] args)
        {
            var stream = (TInput)args[0];
            return Execute(stream);
        }

        /// <summary>
        /// Execution using base strategy
        /// </summary>
        /// <returns></returns>
        internal ListResult<TResponse> Execute(TInput stream)
        {
            var result = Authenticate(stream);
            if (result.IsFaulted) return result;

            result = Authorize(stream);
            if (result.IsFaulted) return result;

            result = Validate(stream);
            if (result.IsFaulted) return result;

            var fileImporter = ResolveFileImporter();
            var importedData = fileImporter.Import(ImportConfiguration, stream);

            return new ListResult<TResponse>(importedData);
        }
    }
}
