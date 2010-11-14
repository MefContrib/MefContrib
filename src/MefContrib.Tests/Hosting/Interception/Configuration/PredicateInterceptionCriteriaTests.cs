using System;
using MefContrib.Hosting.Interception.Configuration;
using NUnit.Framework;

namespace MefContrib.Hosting.Interception.Tests.Configuration
{
    [TestFixture]
    public class PredicateInterceptionCriteriaTests
    {
        [Test]
        public void When_calling_ctor_with_null_interceptor_argument_null_exception_is_thrown()
        {
            Assert.That(delegate
            {
                new PredicateInterceptionCriteria(null, def => true);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void When_calling_ctor_with_null_predicate_argument_null_exception_is_thrown()
        {
            Assert.That(delegate
            {
                new PredicateInterceptionCriteria(new FakeInterceptor(), null);
            }, Throws.TypeOf<ArgumentNullException>());
        }
    }
}