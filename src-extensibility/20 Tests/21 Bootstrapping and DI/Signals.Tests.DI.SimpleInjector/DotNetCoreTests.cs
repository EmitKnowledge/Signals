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
    }
}
