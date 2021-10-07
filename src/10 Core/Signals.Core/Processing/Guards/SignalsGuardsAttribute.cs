using Signals.Core.Processes.Base;
using Signals.Core.Processing.Exceptions;
using System;

namespace Signals.Core.Processing.Guards
{
    /// <summary>
    /// Process guard attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class SignalsGuardAttribute : Attribute
    {
        private ISignalsGuard Guard { get; }
        private object[] Args { get; }

        /// <summary>
        /// CTOR
        /// </summary>
        public SignalsGuardAttribute(Type guardType, params object[] args)
        {
            if (!(Activator.CreateInstance(guardType) is ISignalsGuard guard))
            {
	            this.D($"The guard type: {guardType?.FullName} must implement the ISignalsGuard interface.");
                throw new ArgumentException("The guard must implement the ISignalsGuard interface.");
            }

            Guard = guard;
            Args = args;
        }

        /// <summary>
        /// Authorization callback
        /// </summary>
        /// <returns></returns>
        internal CodeSpecificErrorInfo Check(IBaseProcessContext processContext)
            => Guard.Check(processContext, Args) ? null : Guard.GetCodeSpecificErrorInfo(Args);
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
        bool Check(IBaseProcessContext processContext, object[] args);

        /// <summary>
        /// Represents the error generated if the guard check fails
        /// </summary>
        CodeSpecificErrorInfo GetCodeSpecificErrorInfo(object[] args);
    }
}