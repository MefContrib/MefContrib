namespace MefContrib.Hosting.Interception.Unity.Tests
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using Microsoft.Practices.Unity;
    using Microsoft.Practices.Unity.InterceptionExtension;
    using MefContrib.Hosting.Interception.Configuration;
    using MefContrib.Hosting.Interception.Unity;
    using MefContrib.Tests;
    using NUnit.Framework;

    [TestFixture]
    public class InterfaceInterceptorTests
    {
        private CompositionContainer container;

        [SetUp]
        public void TestSetUp()
        {
            var innerCatalog = new TypeCatalog(typeof(Customer));
            var interceptor = new InterfaceInterceptor();

            var valueInterceptor = new DynamicProxyInterceptor(interceptor);
            var cfg = new InterceptionConfiguration()
                .AddInterceptor(valueInterceptor);

            var catalog = new InterceptingCatalog(innerCatalog, cfg);
            container = new CompositionContainer(catalog);
        }

        [Test]
        public void When_setting_name_on_the_customer_it_should_error()
        {
            var customer = container.GetExportedValue<ICustomer>();
            typeof(InvalidOperationException).ShouldBeThrownBy(() => { customer.Name = "John Doe"; });
        }
    }

    [Export(typeof(ICustomer))]
    public class Customer : ICustomer
    {
        [FailingHandler]
        public string Name { get; set; }
    }

    public interface ICustomer
    {
        string Name { get; set; }
    }

    internal class FailingHandlerAttribute : HandlerAttribute
    {
        public override ICallHandler CreateHandler(IUnityContainer container)
        {
            return new FailingCallHandler();
        }
    }

    internal class FailingCallHandler : ICallHandler
    {
        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
           if (input.MethodBase.Name.StartsWith("set_", StringComparison.OrdinalIgnoreCase))
           {
               throw new InvalidOperationException();
           }
           return getNext()(input, getNext);
        }

        public int Order { get; set; }
    }
}