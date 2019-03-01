using System;

namespace Signals.Core.Processing.Input.Http.ModelBinding
{
    /// <summary>
    /// Represents parameter binding attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class SignalsParameterBindingAttribute : Attribute
    {
        /// <summary>
        /// Represents the parameter binder
        /// </summary>
        public BaseModelBinder Binder { get; }

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="modelBinder"></param>
		public SignalsParameterBindingAttribute(Type modelBinder)
        {
	        Binder = (BaseModelBinder)Activator.CreateInstance(modelBinder);
        }
    }
}
