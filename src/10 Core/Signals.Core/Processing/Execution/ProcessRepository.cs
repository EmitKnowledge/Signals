using Signals.Core.Common.Instance;
using Signals.Core.Common.Reflection;
using Signals.Core.Processes.Base;
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
        private readonly List<Type> _processTypes;

        public ProcessRepository(params Assembly[] scanAssemblies)
        {
            _processTypes = (from x in scanAssemblies.SelectMany(x => x.LoadAllTypesFromAssembly())
                            where ImplementsOpenGenericInterface(x, typeof(IBaseProcess<>))
                            select x)
                            .Where(x => !x.IsInterface && !x.IsAbstract)
                            .Distinct()
                            .ToList();
            this.D($"Initializing process repository. Total process types discovered: {_processTypes.Count}.");
        }

        /// <summary>Determines whether a type, like IList&lt;int&gt;, implements an open generic interface, like
        /// IEnumerable&lt;&gt;. Note that this only checks against *interfaces*.</summary>
        /// <param name="candidateType">The type to check.</param>
        /// <param name="openGenericInterfaceType">The open generic type which it may impelement</param>
        /// <returns>Whether the candidate type implements the open interface.</returns>
        private bool ImplementsOpenGenericInterface(Type candidateType, Type openGenericInterfaceType)
        {
	        return
		        candidateType == openGenericInterfaceType ||
		        (candidateType.IsGenericType && candidateType.GetGenericTypeDefinition() == openGenericInterfaceType) ||
		        candidateType.GetInterfaces().Any(i => i.IsGenericType && ImplementsOpenGenericInterface(i, openGenericInterfaceType));
        }

        /// <summary>
        /// Get process type by interface
        /// </summary>
        /// <param name="type"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<Type> OfType(Type type, Func<Type, bool> filter = null)
        {
            if (type.IsNull()) return null;
            filter = filter ?? new Func<Type, bool>(x => true);
            return _processTypes.Where(x => x.GetInterfaces().Contains(type) && filter(x)).ToList();
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
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<Type> All(Func<Type, bool> filter = null)
        {
            filter = filter ?? new Func<Type, bool>(x => true);
            return _processTypes.Where(filter).ToList();
        }

        /// <summary>
        /// Get process type by name and interface
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public Type First(Func<Type, bool> filter = null)
        {
	        filter = filter ?? new Func<Type, bool>(x => true);
	        return _processTypes.FirstOrDefault(filter);
        }
    }
}