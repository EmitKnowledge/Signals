using System.IO;
using Signals.Aspects.DI.Attributes;
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
        [Import]
        protected virtual IFileImportProcessContext Context
        {
            get => _context;
            set { (value as FileImportProcessContext)?.SetProcess(this); _context = value; }
        }
        private IFileImportProcessContext _context;

        /// <summary>
        /// Base process context override
        /// </summary>
        internal override IBaseProcessContext BaseContext => Context;

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
        /// Authentication and authorization step
        /// </summary>
        /// <returns></returns>
        public abstract ListResult<TResponse> Auth(TInput stream);

        /// <summary>
        /// Validation step
        /// </summary>
        /// <returns></returns>
        public abstract ListResult<TResponse> Validate(TInput stream);
        
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
            var result = Auth(stream);
            this.D("Executed -> Auth.");
            if (result.IsFaulted)
            {
	            this.D("Executed -> Auth -> Failed.");
                return result;
            }

            result = Validate(stream);
            this.D("Executed -> Validate.");
            if (result.IsFaulted)
            {
	            this.D("Executed -> Validation -> Failed.");
                return result;
            }

            var fileImporter = ResolveFileImporter();
            this.D($"File importer resolved.");

            var importedData = fileImporter.Import(ImportConfiguration, stream);
            this.D($"Imported: {importedData.Count} data records from the provided stream.");

            return new ListResult<TResponse>(importedData);
        }
    }
}
