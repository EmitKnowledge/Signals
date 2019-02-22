using System;

namespace Signals.Core.Processing.Input
{
    /// <summary>
    /// Represents parameter binding attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class SignalsParameterBindingAttribute : Attribute
    {
        /// <summary>
        /// Represents the parameter binding
        /// </summary>
        public ParameterBinding ParameterBinding { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="parameterBinding"></param>
        public SignalsParameterBindingAttribute(ParameterBinding parameterBinding)
        {
            ParameterBinding = parameterBinding;
        }
    }
}
