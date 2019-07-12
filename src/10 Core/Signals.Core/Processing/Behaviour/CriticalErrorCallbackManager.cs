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
        private static List<Action<Type, object[]>> Callbacks { get; set; }

        /// <summary>
        /// CTOR
        /// </summary>
        static CriticalErrorCallbackManager()
        {
            Callbacks = new List<Action<Type, object[]>>();
        }

        /// <summary>
        /// Register callback to execute on error happened
        /// </summary>
        /// <param name="callback"></param>
        public void OnError(Action<Type, object[]> callback)
        {
            Callbacks.Add(callback);
        }

        /// <summary>
        /// Invoke callbacks when error happens
        /// </summary>
        /// <param name="processType"></param>
        /// <param name="args"></param>
        internal void InvokeError(Type processType, object[] args)
        {
            foreach(var callback in Callbacks)
            {
                callback(processType, args);
            }
        }
    }
}
