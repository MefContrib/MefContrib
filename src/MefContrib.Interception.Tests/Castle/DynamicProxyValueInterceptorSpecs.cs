using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Text;
using Castle.DynamicProxy;
using Composition.Interception.DynamicProxy;
using MefContrib.Interception;
using MefContrib.Tests;
using NUnit.Framework;

namespace MefContrib.Interception.Castle.Tests
{
    namespace Given_a_customer_is_intercepted_with_a_freezable_interceptor
    {
        [TestFixture]
        public class when_setting_name_on_the_customer : DynamicProxyValueInterceptorContext
        {
            [Test]
            public void it_should_error()
            {
                typeof(InvalidOperationException).ShouldBeThrownBy(() => { Customer.Name = "John Doe"; });
                
            }

            public override void Context()
            {
                Customer = Container.GetExportedValue<ICustomer>();
            }
        }

        public class DynamicProxyValueInterceptorContext
        {
            public ComposablePartCatalog Catalog;
            public CompositionContainer Container;
            public ICustomer Customer;

            public DynamicProxyValueInterceptorContext()
            {
                var innerCatalog = new TypeCatalog(typeof(Customer));
                var interceptor = new FreezableInterceptor();
                interceptor.Freeze();
                var valueInterceptor = new DynamicProxyInterceptor(interceptor);
                Catalog = new InterceptingCatalog(innerCatalog, valueInterceptor);
                Container = new CompositionContainer(Catalog);
                Context();
            }

            public virtual void Context()
            {
            }
        }

        [Export(typeof(ICustomer))]
        public class Customer : ICustomer
        {
            public virtual string Name { get; set; }
        }

        public interface ICustomer
        {
            string Name { get; set; }
        }

        //Freezable interceptor taken from Krzysztof Kozmic
        //
        internal class FreezableInterceptor : IInterceptor, IFreezable
        {
            private bool _isFrozen;

            public void Freeze()
            {
                _isFrozen = true;
            }

            public bool IsFrozen
            {
                get { return _isFrozen; }
            }

            public void Intercept(IInvocation invocation)
            {
                if (_isFrozen && invocation.Method.Name.StartsWith("set_", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException();
                }
                invocation.Proceed();
            }

        }

        internal interface IFreezable { bool IsFrozen { get; } void Freeze();}
    }
}