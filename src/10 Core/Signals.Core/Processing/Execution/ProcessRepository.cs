using Signals.Core.Processes.Base;
using Signals.Core.Common.Instance;
using Signals.Core.Common.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Signals.Core.Processing.Execution
{
    internal class ProcessRepository
    {
        /// <summary>
        /// All process types
        /// </summary>
        private readonly List<Type> ProcessTypes;

        public ProcessRepository(params Assembly[] scanAssemblies)
        {
            ProcessTypes = (from x in scanAssemblies.SelectMany(x => x.LoadAllTypesFromAssembly())
                            where ImplementsOpenGenericInterface(x, typeof(IBaseProcess<>))
                            select x)
                            .Where(x => !x.IsInterface && !x.IsAbstract)
                            .ToList();
        }

        /// <summary>Determines whether a type, like IList&lt;int&gt;, implements an open generic interface, like
        /// IEnumerable&lt;&gt;. Note that this only checks against *interfaces*.</summary>
        /// <param name="candidateType">The type to check.</param>
        /// <param name="openGenericInterfaceType">The open generic type which it may impelement</param>
        /// <returns>Whether the candidate type implements the open interface.</returns>
        private static bool ImplementsOpenGenericInterface(Type candidateType, Type openGenericInterfaceType)
        {
            return
                candidateType == openGenericInterfaceType ||
                (candidateType.IsGenericType && candidateType.GetGenericTypeDefinition() == openGenericInterfaceType) ||
                candidateType.GetInterfaces().Any(i => i.IsGenericType && ImplementsOpenGenericInterface(i, openGenericInterfaceType));
        }

        /// <summary>
        /// Get process type by name
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public Type ResolveProcess(Func<Type, bool> filter = null, params string[] names)
        {
            filter = filter ?? new Func<Type, bool>(x => true);
            return ProcessTypes.Where(filter).FirstOrDefault(x => names.Contains(x.Name, StringComparer.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Get process type by name and interface
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <param name="filter"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public Type ResolveProcess<TInterface>(Func<Type, bool> filter = null, params string[] names)
        {
            filter = filter ?? new Func<Type, bool>(x => true);
            return ProcessTypes.Where(filter).FirstOrDefault(x => x.GetInterfaces().Contains(typeof(TInterface)) && names.Contains(x.Name, StringComparer.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Get process type by interface
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <param name="name"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<Type> OfType(Type type, Func<Type, bool> filter = null)
        {
            if (type.IsNull()) return null;
            filter = filter ?? new Func<Type, bool>(x => true);
            return ProcessTypes.Where(x => x.GetInterfaces().Contains(type) && filter(x)).ToList();
        }

        /// <summary>
        /// Get process type by interface
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<Type> OfType<TInterface>(Func<Type, bool> filter = null)
        {
            filter = filter ?? new Func<Type, bool>(x => true);
            return OfType(typeof(TInterface), filter);
        }

        /// <summary>
        /// Get process type by name and interface
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<Type> All(Func<Type, bool> filter = null)
        {
            filter = filter ?? new Func<Type, bool>(x => true);
            return ProcessTypes.Where(filter).ToList();
        }
    }
}