using Signals.Core.Processes.Base;
using Signals.Core.Processing.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Signals.Core.Processing.Guards
{
    /// <summary>
    /// Process guard attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class SignalsGuardsAttribute : Attribute
    {
        private List<ISignalsGuard> Guards { get; }

        /// <summary>
        /// CTOR
        /// </summary>
        public SignalsGuardsAttribute(params Type[] guardTypes)
        {
            Guards = new List<ISignalsGuard>();

            foreach (var guardType in guardTypes)
            {
                if (!(Activator.CreateInstance(guardType) is ISignalsGuard guard))
                {
                    throw new ArgumentException("All guards must implement the ISignalsGuard interface.");
                }

                Guards.Add(guard);
            }
        }

        /// <summary>
        /// Authorization callback
        /// </summary>
        /// <returns></returns>
        internal CodeSpecificErrorInfo Guard(IBaseProcessContext processContext)
        {
            var failedGuard = Guards.FirstOrDefault(x => !x.Check(processContext));
            return failedGuard?.GetCodeSpecificErrorInfo();
        }
    }

    /// <summary>
    /// Represents process guard
    /// </summary>
    public interface ISignalsGuard
    {
        /// <summary>
        /// Checks if the guard is passed
        /// </summary>
        /// <returns></returns>
        bool Check(IBaseProcessContext processContext);

        /// <summary>
        /// Represents the error generated if the guard check fails
        /// </summary>
        CodeSpecificErrorInfo GetCodeSpecificErrorInfo();
    }
}