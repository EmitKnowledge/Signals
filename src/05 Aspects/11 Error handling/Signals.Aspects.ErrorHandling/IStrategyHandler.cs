﻿using System;
using System.Threading.Tasks;

namespace Signals.Aspects.ErrorHandling
{
	/// <summary>
	/// Execution handler for specific stragety
	/// </summary>
	public interface IStrategyHandler
	{
        /// <summary>
        /// Execute callback wrapped in an execution strategy
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        Task<TResult> Execute<TResult>(Func<TResult> action);
    }
}
