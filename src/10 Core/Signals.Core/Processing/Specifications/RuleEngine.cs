using Signals.Core.Common.Instance;
using Signals.Core.Processing.Exceptions;
using Signals.Core.Processing.Results;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Signals.Core.Processing.Specifications
{
    /// <summary>
    /// Specificaiton executor
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class RuleEngine<TResult>
        where TResult : VoidResult, new()
    {
        /// <summary>
        /// Execution results converted to errors
        /// </summary>
        private List<SpecificationErrorInfo> _specificationErrors;

        /// <summary>
        /// Current execution strategy
        /// </summary>
        private SpecificationExecutionStrategy _strategy;

        /// <summary>
        /// CTOR
        /// </summary>
        public RuleEngine()
        {
            _specificationErrors = new List<SpecificationErrorInfo>();
            _strategy = new StopAtFirstFailedStrategy();
        }

        /// <summary>
        /// Provides execution result
        /// </summary>
        /// <returns></returns>
        public TResult ReturnResult()
        {
            var result = new TResult();
            result.IsSystemFault = false;
            result.IsFaulted = _specificationErrors.Any();
            result.ErrorMessages = _specificationErrors.Cast<IErrorInfo>().ToList();

            return result;
        }

        /// <summary>
        /// Execution strategy setter
        /// </summary>
        /// <param name="strategy"></param>
        /// <returns></returns>
        public RuleEngine<TResult> UseStrategy(SpecificationExecutionStrategy strategy)
        {
            _strategy = strategy;

            return this;
        }

        /// <summary>
        /// Specification execution with input
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="specificaiton"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public RuleEngine<TResult> Validate<T>(BaseSpecification<T> specificaiton, T input)
        {
            var result = _strategy.Execute(specificaiton, input);

            if (!result.IsNull() && !result.IsValid)
            {
                var specResult = new SpecificationErrorInfo(result);
                _specificationErrors.Add(specResult);
            }

            return this;
        }


        /// <summary>
        /// Specification execution with input
        /// </summary>
        /// <param name="specificaiton"></param>
        /// <returns></returns>
        public RuleEngine<TResult> Validate(BaseSpecification specificaiton)
        {
            var result = _strategy.Execute(specificaiton);

            if (!result.IsNull() && !result.IsValid)
            {
                var specResult = new SpecificationErrorInfo(result);
                _specificationErrors.Add(specResult);
            }

            return this;
        }

        /// <summary>
        /// Specification container execution with input
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="specificaiton"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public RuleEngine<TResult> Validate<T>(BaseSpecificationContainer<T> specificaiton, T input)
        {
            specificaiton.Use(_strategy);
            var result = specificaiton.Execute(input);

            if (result.Any(x => !x.IsValid))
            {
                _specificationErrors.AddRange(result.Where(x => !x.IsValid).Select(x => new SpecificationErrorInfo(x)));
            }

            return this;
        }

        /// <summary>
        /// Specification container execution with input
        /// </summary>
        /// <param name="specificaiton"></param>
        /// <returns></returns>
        public RuleEngine<TResult> Validate(BaseSpecificationContainer specificaiton)
        {
            specificaiton.Use(_strategy);
            var result = specificaiton.Execute();

            if (result.Any(x => !x.IsValid))
            {
                _specificationErrors.AddRange(result.Where(x => !x.IsValid).Select(x => new SpecificationErrorInfo(x)));
            }

            return this;
        }

        /// <summary>
        /// Specification execution with input
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition"></param>
        /// <param name="specificaiton"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public RuleEngine<TResult> ValidateIf<T>(Func<bool> condition, BaseSpecification<T> specificaiton, T input)
        {
            if (!condition()) return this;
            return Validate(specificaiton, input);
        }

        /// <summary>
        /// Specification execution with input
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="specificaiton"></param>
        /// <returns></returns>
        public RuleEngine<TResult> ValidateIf(Func<bool> condition, BaseSpecification specificaiton)
        {
            if (!condition()) return this;
            return Validate(specificaiton);
        }

        /// <summary>
        /// Specification execution with input
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition"></param>
        /// <param name="specificaiton"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public RuleEngine<TResult> ValidateIf<T>(Func<bool> condition, BaseSpecificationContainer<T> specificaiton, T input)
        {
            if (!condition()) return this;
            return Validate(specificaiton, input);
        }

        /// <summary>
        /// Specification execution with input
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="specificaiton"></param>
        /// <returns></returns>
        public RuleEngine<TResult> ValidateIf(Func<bool> condition, BaseSpecificationContainer specificaiton)
        {
            if (!condition()) return this;
            return Validate(specificaiton);
        }
    }
}