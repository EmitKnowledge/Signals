using Signals.Aspects.DI;
using Signals.Core.Common.Instance;
using System.Collections.Generic;
using System.Linq;

namespace Signals.Core.Processing.Specifications
{
    /// <summary>
    /// Specificaiton container that presents macro for several specifications
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseSpecificationContainer<T>
    {
        /// <summary>
        /// Internal specificaitons
        /// </summary>
        private List<BaseSpecification<T>> _specificaitons;

        /// <summary>
        /// Execution strategy
        /// </summary>
        private SpecificationExecutionStrategy _strategy;

        /// <summary>
        /// CTOR
        /// </summary>
        public BaseSpecificationContainer()
        {
            SystemBootstrapper.Bootstrap(this);
            _specificaitons = new List<BaseSpecification<T>>();
        }

        /// <summary>
        /// Adds specification to the container to be executed
        /// </summary>
        /// <param name="specification"></param>
        public void Add(BaseSpecification<T> specification)
        {
            _specificaitons.Add(specification);
        }

        /// <summary>
        /// Execute all internal specificaitons using the provided strategy
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        internal List<SpecificationResult> Execute(T input)
        {
            return _specificaitons.Select(x => _strategy.Execute(x, input)).Where(x => !x.IsNull()).ToList();
        }

        /// <summary>
        /// Execution strategy setter
        /// </summary>
        /// <param name="strategy"></param>
        internal void Use(SpecificationExecutionStrategy strategy)
        {
            _strategy = strategy;
        }
    }

    /// <summary>
    /// Specificaiton container that presents macro for several specifications
    /// </summary>
    public abstract class BaseSpecificationContainer
    {
        /// <summary>
        /// Internal specificaitons
        /// </summary>
        private List<BaseSpecification> _specificaitons;

        /// <summary>
        /// Execution strategy
        /// </summary>
        private SpecificationExecutionStrategy _strategy;

        /// <summary>
        /// CTOR
        /// </summary>
        public BaseSpecificationContainer()
        {
            SystemBootstrapper.Bootstrap(this);
            _specificaitons = new List<BaseSpecification>();
        }

        /// <summary>
        /// Adds specification to the container to be executed
        /// </summary>
        /// <param name="specification"></param>
        public void Add(BaseSpecification specification)
        {
            _specificaitons.Add(specification);
        }

        /// <summary>
        /// Execute all internal specificaitons using the provided strategy
        /// </summary>
        /// <returns></returns>
        internal List<SpecificationResult> Execute()
        {
            return _specificaitons.Select(x => _strategy.Execute(x)).Where(x => !x.IsNull()).ToList();
        }

        /// <summary>
        /// Execution strategy setter
        /// </summary>
        /// <param name="strategy"></param>
        internal void Use(SpecificationExecutionStrategy strategy)
        {
            _strategy = strategy;
        }
    }
}