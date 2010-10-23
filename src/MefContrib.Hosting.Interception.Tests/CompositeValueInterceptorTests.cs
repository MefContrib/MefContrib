using System;
using NUnit.Framework;

namespace MefContrib.Hosting.Interception.Tests
{
    [TestFixture]
    public class CompositeValueInterceptorTests
    {
        [Test]
        public void Interceptors_are_evaluated_in_order_they_are_added()
        {
            var compositeValueInterceptor = new CompositeValueInterceptor();
            compositeValueInterceptor.Interceptors.Add(new Wrapper1Interceptor());
            compositeValueInterceptor.Interceptors.Add(new Wrapper2Interceptor());

            var val = "this is a value";
            var interceptedValue = (IWrapper) compositeValueInterceptor.Intercept(val);

            Assert.That(interceptedValue.GetType(), Is.EqualTo(typeof(Wrapper2)));
            Assert.That(interceptedValue.Value.GetType(), Is.EqualTo(typeof(Wrapper1)));
            Assert.That(((IWrapper)interceptedValue.Value).Value, Is.EqualTo(val));
        }

        public interface IWrapper
        {
            object Value { get; set; }
        }

        public class Wrapper1 : IWrapper
        {
            public Wrapper1(object value)
            {
                Value = value;
            }

            public object Value { get; set; }
        }

        public class Wrapper2 : IWrapper
        {
            public Wrapper2(object value)
            {
                Value = value;
            }

            public object Value { get; set; }
        }

        public class Wrapper1Interceptor : IExportedValueInterceptor
        {
            public object Intercept(object value)
            {
                return new Wrapper1(value);
            }
        }

        public class Wrapper2Interceptor : IExportedValueInterceptor
        {
            public object Intercept(object value)
            {
                return new Wrapper2(value);
            }
        }
    }
}