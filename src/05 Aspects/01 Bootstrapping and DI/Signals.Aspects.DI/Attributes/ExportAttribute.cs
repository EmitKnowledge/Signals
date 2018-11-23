using System;

namespace Signals.Aspects.DI.Attributes
{
    /// <summary>
    /// Decorates classes that are concrete implementation of defined interface @DefinitionType
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ExportAttribute : Attribute
    {
        /// <summary>
        /// Service type to be instanced as implementation
        /// </summary>
        public Type DefinitionType { get; set; }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="definitionType"></param>
        public ExportAttribute(Type definitionType)
        {
            // if @definitionType is  not interface, throw
            if (!definitionType.IsInterface) throw new InvalidCastException("Interface expected");

            DefinitionType = definitionType;
        }
    }
}
