using Signals.Core.Processes.Base;
using Signals.Core.Processing.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Signals.Core.Processing.Behaviour
{
    /// <summary>
    /// Critical error manager
    /// </summary>
    public class CriticalErrorCallbackManager
    {
        /// <summary>
        /// Registered callbacks
        /// </summary>
        private static List<Action<IBaseProcess<VoidResult>, Type, object[], Exception>> Callbacks { get; set; }

        /// <summary>
        /// CTOR
        /// </summary>
        static CriticalErrorCallbackManager()
        {
            Callbacks = new List<Action<IBaseProcess<VoidResult>, Type, object[], Exception>>();
        }

        /// <summary>
        /// Register callback to execute on error happened
        /// </summary>
        /// <param name="callback"></param>
        public void OnError(Action<IBaseProcess<VoidResult>, Type, object[], Exception> callback)
        {
            Callbacks.Add(callback);
        }

        /// <summary>
        /// Invoke callbacks when error happens
        /// </summary>
        /// <param name="process"></param>
        /// <param name="processType"></param>
        /// <param name="args"></param>
        /// <param name="ex"></param>
        internal void InvokeError(IBaseProcess<VoidResult> process, Type processType, object[] args, Exception ex)
        {
            foreach(var callback in Callbacks)
            {
                callback(process, processType, args, ex);
                this.D($"Executed callback for process type: {processType?.FullName}. Exception: {ex?.Message}");
            }
        }
    }
}
