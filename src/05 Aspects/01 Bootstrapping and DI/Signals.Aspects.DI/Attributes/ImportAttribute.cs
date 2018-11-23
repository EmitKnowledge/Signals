using System;

namespace Signals.Aspects.DI.Attributes
{
    /// <summary>
    /// Decorates properties that need to be injected with a concrete instance of their service type
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ImportAttribute : Attribute
    {
    }
}
