namespace Signals.Core.Processing.Specifications
{
    /// <summary>
    /// Specificaiton execution strategy
    /// </summary>
    public abstract class SpecificationExecutionStrategy
    {
        /// <summary>
        /// Specificaiton execution using this strategy
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="specification"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public abstract SpecificationResult Execute<T>(BaseSpecification<T> specification, T input);
    }

    /// <summary>
    /// Strategy that executes specificaitons until first validation fail
    /// </summary>
    public class StopAtFirstFailedStrategy : SpecificationExecutionStrategy
    {
        /// <summary>
        /// Flag to indicate if the execution shouyld be stopped
        /// </summary>
        internal bool ExecuteNext { get; set; }

        /// <summary>
        /// CTOR
        /// </summary>
        public StopAtFirstFailedStrategy()
        {
            ExecuteNext = true;
        }

        /// <summary>
        /// Specificaiton execution using this strategy
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="specification"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public override SpecificationResult Execute<T>(BaseSpecification<T> specification, T input)
        {
            if (ExecuteNext)
            {
                var result = specification.Execute(input);
                if (!result.IsValid)
                {
                    ExecuteNext = false;
                }
                return result;
            }

            return null;
        }
    }

    /// <summary>
    /// Strategy that executes all specificaitons no matter the result
    /// </summary>
    public class ExecuteAllStrategy : SpecificationExecutionStrategy
    {
        /// <summary>
        /// Specificaiton execution using this strategy
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="specification"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public override SpecificationResult Execute<T>(BaseSpecification<T> specification, T input)
        {
            return specification.Execute(input);
        }
    }
}