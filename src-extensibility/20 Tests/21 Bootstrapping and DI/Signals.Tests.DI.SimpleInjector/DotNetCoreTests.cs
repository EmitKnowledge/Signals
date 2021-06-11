using Signals.Aspects.DI;
using Signals.Aspects.DI.Attributes;
using Signals.Aspects.DI.DotNetCore;
using Signals.Tests.DI.Services;
using Signals.Tests.DI.Services.Impl;
using System;
using Xunit;

namespace Signals.Tests.DI
{
    public class DotNetCoreTests
    {
        [Import] public IMyService1 MyService1 { get; set; }

        [Fact]
        public void MyService_WhenInjected_ReturnsValue()
        {
            var service = new RegistrationService();

            SystemBootstrapper.Init(service, typeof(DotNetCoreTests).Assembly);
            SystemBootstrapper.Bootstrap(this);

            Assert.Equal(3, MyService1.Add(1, 2));
        }

        [Fact]
        public void MyService_WhenRegistered_InstantiatedAsInterface_ReturnsValue()
        {
            var service = new RegistrationService();

            SystemBootstrapper.Init(service, typeof(DotNetCoreTests).Assembly);

            var srv = SystemBootstrapper.GetInstance<IMyService1>();
            Assert.Equal(3, srv.Add(1, 2));
        }

        [Fact]
        public void MyService_WhenRegistered_InstantiatedAsConcrete_ReturnsValue()
        {
            var service = new RegistrationService();
            service.Register<MyService1>();

            SystemBootstrapper.Init(service, typeof(DotNetCoreTests).Assembly);

            var srv = SystemBootstrapper.GetInstance<MyService1>();
            Assert.Equal(3, srv.Add(1, 2));
        }

        [Fact]
        public void MyService_WhenRegistered_InstantiatedAsConcreteSingleton_ReturnsValue()
        {
            var service = new RegistrationService();
            service.RegisterSingleton<MyService1>();
            service.RegisterSingleton<IMyService2, MyService2>();
            service.RegisterSingleton<IMyService3, MyService3>();
            service.RegisterSingleton<IMyService4, MyService4>();

            SystemBootstrapper.Init(service);

            var srv1 = SystemBootstrapper.GetInstance<MyService1>();
            var srv2 = SystemBootstrapper.GetInstance<MyService1>();
            Assert.Equal(3, srv1.Add(1, 2));
            Assert.Equal(srv2, srv1);
        }

        [Fact]
        public void MyService_WhenRegistered_InstantiatedAsConcreteTransient_ReturnsValue()
        {
            var service = new RegistrationService();
            service.Register<MyService1>();
            service.RegisterSingleton<IMyService2, MyService2>();
            service.RegisterSingleton<IMyService3, MyService3>();
            service.RegisterSingleton<IMyService4, MyService4>();

            SystemBootstrapper.Init(service);

            var srv1 = SystemBootstrapper.GetInstance<MyService1>();
            var srv2 = SystemBootstrapper.GetInstance<MyService1>();
            Assert.Equal(3, srv1.Add(1, 2));
            Assert.NotEqual(srv2, srv1);
        }

        [Fact]
        public void MyService_WhenRegisteredTwice_RegistrationIsOverriden()
        {
            var service = new RegistrationService();
            service.RegisterSingleton<IMyService1, MyService1>();
            service.RegisterSingleton<IMyService1, MyService1Override>();
            service.RegisterSingleton<IMyService2, MyService2>();
            service.RegisterSingleton<IMyService3, MyService3>();
            service.RegisterSingleton<IMyService4, MyService4>();

            SystemBootstrapper.Init(service);

            var srv1 = SystemBootstrapper.GetInstance<IMyService1>();
            Assert.Equal(4, srv1.Add(1, 2));
        }
    }
}
