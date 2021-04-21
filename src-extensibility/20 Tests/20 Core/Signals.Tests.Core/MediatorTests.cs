using Signals.Aspects.Caching.InMemory.Configurations;
using Signals.Aspects.Configuration.File;
using Signals.Aspects.DI;
using Signals.Aspects.DI.Attributes;
using Signals.Aspects.Localization;
using Signals.Core.Common.Instance;
using Signals.Core.Configuration;
using Signals.Core.Configuration.Bootstrapping;
using Signals.Core.Processes;
using Signals.Core.Processes.Business;
using Signals.Core.Processing.Results;
using System;
using System.IO;
using Xunit;

namespace Signals.Tests.Core
{
    public class MediatorTests
    {
        private interface INumberProcessor
        {
            int Proc(int number);
        }

        [Export(typeof(INumberProcessor))]
        private class IProcessor : INumberProcessor
        {
            public int Proc(int number)
            {
                return number;
            }
        }

        private interface IIncrement
        {
            int Inc(int number);
        }

        [Export(typeof(IIncrement))]
        private class Increment : IIncrement
        {
            [Import] private INumberProcessor NumberProcessor { get; set; }

            public int Inc(int number)
            {
                return NumberProcessor.Proc(number + 1);
            }
        }

        private interface IDecrement
        {
            int Dec(int number);
        }

        [Export(typeof(IDecrement))]
        private class Decrement : IDecrement
        {
            [Import] private INumberProcessor NumberProcessor { get; set; }
            [Import] private IIncrement Increment { get; set; }

            public int Dec(int number)
            {
                return NumberProcessor.Proc(Increment.Inc(number) - 2);
            }
        }

        private interface IDouble
        {
            int Dub(int number);
        }

        [Export(typeof(IDouble))]
        private class Double : IDouble
        {
            [Import] private INumberProcessor NumberProcessor { get; set; }
            [Import] private IIncrement Increment { get; set; }
            private IDecrement Decrement { get; set; }

            public Double(IDecrement decrement)
            {
                Decrement = decrement;
            }

            public int Dub(int number)
            {
                var result = number;

                if (number < 0)
                {
                    result = Increment.Inc(result);
                    for (int i = 0; i < number * -1; i++)
                    {
                        result = Decrement.Dec(result);
                    }
                    result = Decrement.Dec(result);
                }
                else
                {
                    result = Decrement.Dec(result);
                    for (int i = 0; i < number; i++)
                    {
                        result = Increment.Inc(result);
                    }
                    result = Increment.Inc(result);
                }

                return NumberProcessor.Proc(result);
            }
        }

        private class DecrementProc : BusinessProcess<int, MethodResult<int>>
        {
            [Import] private IDecrement Decrement { get; set; }

            public override MethodResult<int> Auth(int i) => Ok();
            public override MethodResult<int> Validate(int i) => Ok();

            public override MethodResult<int> Handle(int i)
            {
                Context.Cache.Set("result", i);

                return Decrement.Dec(i);
            }
        }

        private class IncrementProc : BusinessProcess<int, MethodResult<int>>
        {
            [Import] private IIncrement Increment { get; set; }
            private IIncrement Increment2 { get; set; }
            private IDouble Double { get; set; }

            public override MethodResult<int> Auth(int i) => Ok();
            public override MethodResult<int> Validate(int i) => Ok();

            public override MethodResult<int> Handle(int i)
            {
                Context.Cache.Set("result", i);

                if (Increment2.IsNull() && Double.IsNull())
                    return Increment.Inc(i);
                return i;
            }
        }

        private class DoubleProc : BusinessProcess<int, MethodResult<int>>
        {
            [Import] private IDouble Double { get; set; }

            public override MethodResult<int> Auth(int i) => Ok();
            public override MethodResult<int> Validate(int i) => Ok();

            public override MethodResult<int> Handle(int i)
            {
                Context.Cache.Set("result", i);

                return Double.Dub(i);
            }
        }


        private class IncrementAndDoubleProc : BusinessProcess<int, MethodResult<int>>
        {
            public override MethodResult<int> Auth(int i) => Ok();
            public override MethodResult<int> Validate(int i) => Ok();

            public override MethodResult<int> Handle(int i)
            {
                var incrementResult = Continue<IncrementProc>().With(i);
                var doubleResult = ContinueWith<DoubleProc, int, MethodResult<int>>(incrementResult.Result);

                Context.Cache.Set("result", doubleResult);

                return doubleResult;
            }
        }


        private class DecrementAndDoubleProc : BusinessProcess<int, MethodResult<int>>
        {
            public override MethodResult<int> Auth(int i) => Ok();
            public override MethodResult<int> Validate(int i) => Ok();

            public override MethodResult<int> Handle(int i)
            {
                var decrementResult = Continue<DecrementProc>().With(i);
                var doubleResult = ContinueWith<DoubleProc, int, MethodResult<int>>(decrementResult);

                Context.Cache.Set("result", doubleResult);

                return doubleResult;
            }
        }

        /// <summary>
        /// Mediator
        /// </summary>
        private Mediator Mediator => SystemBootstrapper.GetInstance<Mediator>();

        public class Config : ApplicationBootstrapConfiguration { }

        private void InitAutofacConfig() => Init(new Aspects.DI.Autofac.RegistrationService());
        private void InitSimpleInjectorConfig() => Init(new Aspects.DI.SimpleInjector.RegistrationService());
        private void InitDotNetConfig() => Init(new Aspects.DI.DotNetCore.RegistrationService());

        private void Init(IRegistrationService registrationService)
        {
            ApplicationConfiguration.UseProvider(new FileConfigurationProvider
            {
                Path = Path.Combine(AppContext.BaseDirectory, "config"),
                File = "app.json"
            });

            var config = new Config();
            config.RegistrationService = registrationService;
            config.CacheConfiguration = new InMemoryCacheConfiguration();
            config.Bootstrap(typeof(MediatorTests).Assembly);
        }

        [Fact]
        public void SimpleInjector1()
        {
            InitSimpleInjectorConfig();

            var startNumber = 1;
            var expectedNumber = 2;

            var increment1 = Mediator.Dispatch<IncrementProc, int, MethodResult<int>>(startNumber);
            var increment2 = Mediator.For<IncrementProc>().With(startNumber);

            Assert.Equal(expectedNumber, increment1.Result);
            Assert.Equal(expectedNumber, increment2.Result);
        }

        [Fact]
        public void SimpleInjector2()
        {
            InitSimpleInjectorConfig();

            var startNumber = 1;
            var expectedNumber = 2;

            var increment1 = Mediator.Dispatch<DoubleProc, int, MethodResult<int>>(startNumber);
            var increment2 = Mediator.For<DoubleProc>().With(startNumber);

            Assert.Equal(expectedNumber, increment1.Result);
            Assert.Equal(expectedNumber, increment2.Result);
        }

        [Fact]
        public void SimpleInjector3()
        {
            InitSimpleInjectorConfig();

            var startNumber = 1;
            var expectedNumber = 4;

            var increment1 = Mediator.Dispatch<IncrementAndDoubleProc, int, MethodResult<int>>(startNumber);
            var increment2 = Mediator.For<IncrementAndDoubleProc>().With(startNumber);

            Assert.Equal(expectedNumber, increment1.Result);
            Assert.Equal(expectedNumber, increment2.Result);
        }

        [Fact]
        public void SimpleInjector4()
        {
            InitSimpleInjectorConfig();

            var startNumber = -1;
            var expectedNumber = -4;

            var increment1 = Mediator.Dispatch<DecrementAndDoubleProc, int, MethodResult<int>>(startNumber);
            var increment2 = Mediator.For<DecrementAndDoubleProc>().With(startNumber);

            Assert.Equal(expectedNumber, increment1.Result);
            Assert.Equal(expectedNumber, increment2.Result);
        }

        [Fact]
        public void DotNet1()
        {
            InitDotNetConfig();

            var startNumber = 1;
            var expectedNumber = 2;

            var increment1 = Mediator.Dispatch<IncrementProc, int, MethodResult<int>>(startNumber);
            var increment2 = Mediator.For<IncrementProc>().With(startNumber);

            Assert.Equal(expectedNumber, increment1.Result);
            Assert.Equal(expectedNumber, increment2.Result);
        }

        [Fact]
        public void DotNet2()
        {
            InitDotNetConfig();

            var startNumber = 1;
            var expectedNumber = 2;

            var increment1 = Mediator.Dispatch<DoubleProc, int, MethodResult<int>>(startNumber);
            var increment2 = Mediator.For<DoubleProc>().With(startNumber);

            Assert.Equal(expectedNumber, increment1.Result);
            Assert.Equal(expectedNumber, increment2.Result);
        }

        [Fact]
        public void DotNet3()
        {
            InitDotNetConfig();

            var startNumber = 1;
            var expectedNumber = 4;

            var increment1 = Mediator.Dispatch<IncrementAndDoubleProc, int, MethodResult<int>>(startNumber);
            var increment2 = Mediator.For<IncrementAndDoubleProc>().With(startNumber);

            Assert.Equal(expectedNumber, increment1.Result);
            Assert.Equal(expectedNumber, increment2.Result);
        }

        [Fact]
        public void DotNet4()
        {
            InitDotNetConfig();

            var startNumber = -1;
            var expectedNumber = -4;

            var increment1 = Mediator.Dispatch<DecrementAndDoubleProc, int, MethodResult<int>>(startNumber);
            var increment2 = Mediator.For<DecrementAndDoubleProc>().With(startNumber);

            Assert.Equal(expectedNumber, increment1.Result);
            Assert.Equal(expectedNumber, increment2.Result);
        }

        [Fact]
        public void Autofac1()
        {
            InitAutofacConfig();

            var startNumber = 1;
            var expectedNumber = 2;

            var increment1 = Mediator.Dispatch<IncrementProc, int, MethodResult<int>>(startNumber);
            var increment2 = Mediator.For<IncrementProc>().With(startNumber);

            Assert.Equal(expectedNumber, increment1.Result);
            Assert.Equal(expectedNumber, increment2.Result);
        }

        [Fact]
        public void Autofac2()
        {
            InitAutofacConfig();

            var startNumber = 1;
            var expectedNumber = 2;

            var increment1 = Mediator.Dispatch<DoubleProc, int, MethodResult<int>>(startNumber);
            var increment2 = Mediator.For<DoubleProc>().With(startNumber);

            Assert.Equal(expectedNumber, increment1.Result);
            Assert.Equal(expectedNumber, increment2.Result);
        }

        [Fact]
        public void Autofac3()
        {
            InitAutofacConfig();

            var startNumber = 1;
            var expectedNumber = 4;

            var increment1 = Mediator.Dispatch<IncrementAndDoubleProc, int, MethodResult<int>>(startNumber);
            var increment2 = Mediator.For<IncrementAndDoubleProc>().With(startNumber);

            Assert.Equal(expectedNumber, increment1.Result);
            Assert.Equal(expectedNumber, increment2.Result);
        }

        [Fact]
        public void Autofac4()
        {
            InitAutofacConfig();

            var startNumber = -1;
            var expectedNumber = -4;

            var increment1 = Mediator.Dispatch<DecrementAndDoubleProc, int, MethodResult<int>>(startNumber);
            var increment2 = Mediator.For<DecrementAndDoubleProc>().With(startNumber);

            Assert.Equal(expectedNumber, increment1.Result);
            Assert.Equal(expectedNumber, increment2.Result);
        }
    }
}