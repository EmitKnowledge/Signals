using SimpleInjector;
using SimpleInjector.Advanced;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Signals.Aspects.DI.SimpleInjector
{
    internal class MostResolvableConstructorBehavior : IConstructorResolutionBehavior
    {
        private readonly Container container;

        public MostResolvableConstructorBehavior(Container container)
        {
            this.container = container;
        }

        private bool IsCalledDuringRegistrationPhase => !this.container.IsLocked;

        [DebuggerStepThrough]
        public ConstructorInfo TryGetConstructor(Type type, out string errorMessage)
        {
            var constructor = this.GetConstructors(type).FirstOrDefault();
            errorMessage = constructor == null ? BuildExceptionMessage(type) : null;
            return constructor;
        }

        private IEnumerable<ConstructorInfo> GetConstructors(Type implementation) =>
            from ctor in implementation.GetConstructors()
            let parameters = ctor.GetParameters()
            where this.IsCalledDuringRegistrationPhase
                || implementation.GetConstructors().Length == 1
                || ctor.GetParameters().All(this.CanBeResolved)
            orderby parameters.Length descending
            select ctor;

        private bool CanBeResolved(ParameterInfo parameter) =>
            this.GetInstanceProducerFor(new InjectionConsumerInfo(parameter)) != null;

        private InstanceProducer GetInstanceProducerFor(InjectionConsumerInfo i) =>
            this.container.Options.DependencyInjectionBehavior.GetInstanceProducer(i, false);

        private static string BuildExceptionMessage(Type type) =>
            !type.GetConstructors().Any()
                ? TypeShouldHaveAtLeastOnePublicConstructor(type)
                : TypeShouldHaveConstructorWithResolvableTypes(type);

        private static string TypeShouldHaveAtLeastOnePublicConstructor(Type type) =>
            string.Format(CultureInfo.InvariantCulture,
                "For the container to be able to create {0}, it should contain at least " +
                "one public constructor.", type.ToFriendlyName());

        private static string TypeShouldHaveConstructorWithResolvableTypes(Type type) =>
            string.Format(CultureInfo.InvariantCulture,
                "For the container to be able to create {0}, it should contain a public " +
                "constructor that only contains parameters that can be resolved.",
                type.ToFriendlyName());
    }
}
