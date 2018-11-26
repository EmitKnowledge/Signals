using System;
using System.Reflection;

namespace Signals.Aspects.Localization.InMemory.Helpers
{
    public static class ReflectionHelper
    {
        /// <summary>
        /// Get path of the current running assemlby
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentAssemblyRunPath()
            => IoHelper.GetPathWithoutFileName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
    }
}
