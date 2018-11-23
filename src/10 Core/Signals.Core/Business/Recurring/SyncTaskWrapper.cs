﻿using Signals.Aspects.BackgroundProcessing;
using Signals.Aspects.Bootstrap;
using Signals.Core.Business.Base;
using Signals.Core.Processing.Execution;
using Signals.Core.Processing.Results;
using System;

namespace Signals.Core.Business.Recurring
{
    /// <summary>
    /// Wrapper for recurring processes to adapt to <see cref="ISyncTask"/>
    /// </summary>
    public class SyncTaskWrapper : ISyncTask
    {
        /// <summary>
        /// Wrapper process
        /// </summary>
        internal readonly Type InnerProcessType;

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="process"></param>
        public SyncTaskWrapper(Type processType)
        {
            InnerProcessType = processType;
        }

        /// <summary>
        /// <see cref="ISyncTask"/> execution handle
        /// </summary>
        public void Execute()
        {
            var executor = SystemBootstrapper.GetInstance<IProcessExecutor>();
            var instance = SystemBootstrapper.GetInstance(InnerProcessType) as IBaseProcess<VoidResult>;

            SystemBootstrapper.Bootstrap(instance);

            executor.Execute(instance);
        }
    }
}