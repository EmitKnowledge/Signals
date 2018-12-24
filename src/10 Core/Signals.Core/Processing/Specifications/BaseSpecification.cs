using Signals.Aspects.DI;

namespace Signals.Core.Processing.Specifications
{
    /// <summary>
    /// Specification definition
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseSpecification<T>
    {
        /// <summary>
        /// CTOR
        /// </summary>
        public BaseSpecification()
        {
            SystemBootstrapper.Bootstrap(this);
        }

        /// <summary>
        /// Execution handle that is called from the <see cref="RuleEngine{TResult}"/>
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        internal SpecificationResult Execute(T input)
        {
            var result = Validate(input);
            var specResult = new SpecificationResult(GetType(), result, input);

            return specResult;
        }

        /// <summary>
        /// Validation handle
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public abstract bool Validate(T input);
    }

    /// <summary>
    /// Specification definition
    /// </summary>
    public abstract class BaseSpecification
    {
        /// <summary>
        /// CTOR
        /// </summary>
        public BaseSpecification()
        {
            SystemBootstrapper.Bootstrap(this);
        }

        /// <summary>
        /// Execution handle that is called from the <see cref="RuleEngine{TResult}"/>
        /// </summary>
        /// <returns></returns>
        internal SpecificationResult Execute()
        {
            var result = Validate();
            var specResult = new SpecificationResult(GetType(), result);

            return specResult;
        }

        /// <summary>
        /// Validation handle
        /// </summary>
        /// <returns></returns>
        public abstract bool Validate();
    }
}