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
using Signals.Aspects.Localization;
using Signals.Core.Common.Instance;

namespace Signals.Core.Processes.Base
{
    /// <summary>
    /// Process context for all types of processes
    /// </summary>
    public interface IBaseProcessContext
    {
        /// <summary>
        /// Logger
        /// </summary>
        ILogger Logger { get;}

        /// <summary>
        /// Auditing provider
        /// </summary>
        IAuditProvider AuditProvider { get; }

        /// <summary>
        /// Authentication manager
        /// </summary>
        IAuthenticationManager Authentication { get;}

        /// <summary>
        /// Authorization manager
        /// </summary>
        IAuthorizationManager Authorization { get; }

        /// <summary>
        /// Storage provider
        /// </summary>
        IStorageProvider Storage { get; }

        /// <summary>
        /// Cache
        /// </summary>
        ICache Cache { get; }

        /// <summary>
        /// Permission manager
        /// </summary>
        IPermissionManager PermissionManager { get; }

        /// <summary>
        /// Localization provider
        /// </summary>
        ILocalizationProvider LocalizationProvider { get; }

        /// <summary>
        /// Process benchmark engine
        /// </summary>
        ProcessBenchmarker Benchmarker { get; }

        /// <summary>
        /// Current user principal
        /// </summary>
        ClaimsPrincipal CurrentUserPrincipal { get; }

        /// <summary>
        /// Mediator
        /// </summary>
        Mediator Mediator { get; }
    }

    /// <summary>
    /// Process context for all types of processes
    /// </summary>
    [Export(typeof(IBaseProcessContext))]
    public class BaseProcessContext : IBaseProcessContext
    {
        private IBaseProcess<VoidResult> process;

        /// <summary>
        /// CTOR
        /// </summary>
        internal void SetProcess(IBaseProcess<VoidResult> process)
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
        /// Localization provider
        /// </summary>
        [Import] public ILocalizationProvider LocalizationProvider { get; internal set; }
        
        /// <summary>
        /// Mediator
        /// </summary>
        [Import] public Mediator Mediator { get; internal set; }

        /// <summary>
        /// Benchmarker
        /// </summary>
        [Import] public IBenchmarker InternalBenchmarker { get; set; }

        /// <summary>
        /// Process benchmark engine
        /// </summary>
        public ProcessBenchmarker Benchmarker => !InternalBenchmarker.IsNull() ? new ProcessBenchmarker(InternalBenchmarker, process) : null;

        /// <summary>
        /// Current user principal
        /// </summary>
        public ClaimsPrincipal CurrentUserPrincipal => Authentication?.GetCurrentPrincipal();
    }
}