using Signals.Aspects.Auditing;
using Signals.Aspects.Auth;
using Signals.Aspects.Caching;
using Signals.Aspects.DI.Attributes;
using Signals.Aspects.Logging;
using Signals.Aspects.Storage;
using Signals.Core.Processing.Execution;
using System;
using System.Security.Claims;
using Signals.Core.Processing.Results;
using Signals.Aspects.Benchmarking;
using Signals.Core.Processing.Benchmarking;
using Signals.Aspects.DI;

namespace Signals.Core.Processes.Base
{
    /// <summary>
    /// Process context for all types of processes
    /// </summary>
    public class BaseProcessContext
    {
        private readonly IBaseProcess<VoidResult> process;

        /// <summary>
        /// CTOR
        /// </summary>
        public BaseProcessContext(IBaseProcess<VoidResult> process)
        {
            this.process = process;
        }

        /// <summary>
        /// Logger
        /// </summary>
        [Import] public ILogger Logger { get; internal set; }

        /// <summary>
        /// Auditing provider
        /// </summary>
        [Import] public IAuditProvider AuditProvider { get; internal set; }

        /// <summary>
        /// Authentication manager
        /// </summary>
        [Import] public IAuthenticationManager Authentication { get; internal set; }

        /// <summary>
        /// Authorization manager
        /// </summary>
        [Import] public IAuthorizationManager Authorization { get; internal set; }

        /// <summary>
        /// Storage provider
        /// </summary>
        [Import] public IStorageProvider Storage { get; internal set; }

        /// <summary>
        /// Cache
        /// </summary>
        [Import] public ICache Cache { get; internal set; }

        /// <summary>
        /// Permission manager
        /// </summary>
        [Import] public IPermissionManager PermissionManager { get; internal set; }

        /// <summary>
        /// Process factory
        /// </summary>
        [Import] internal IProcessFactory ProcessFactory { get; set; }

        /// <summary>
        /// Process executor
        /// </summary>
        [Import] internal IProcessExecutor ProcessExecutor { get; set; }

        /// <summary>
        /// Benchmark engine
        /// </summary>
        public ProcessBenchmarker Benchmarker => new ProcessBenchmarker(SystemBootstrapper.GetInstance<IBenchmarker>(), process);

        /// <summary>
        /// Current user principal
        /// </summary>
        public ClaimsPrincipal CurrentUserPrincipal => Authentication?.GetCurrentPrincipal();
    }
}