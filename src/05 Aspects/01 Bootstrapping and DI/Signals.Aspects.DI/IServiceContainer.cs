﻿using System;

namespace Signals.Aspects.DI
{
    /// <summary>
    /// Dependency resolver
    /// </summary>
    public interface IServiceContainer
    {
        /// <summary>
        /// Get instance of type
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        TService GetInstance<TService>() where TService : class;

        /// <summary>
        /// Get instance of type
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        object GetInstance(Type serviceType);
    }
}
