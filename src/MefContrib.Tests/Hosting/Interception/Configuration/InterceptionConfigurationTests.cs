using System;
using System.Linq;
using MefContrib.Hosting.Generics;
using MefContrib.Hosting.Interception.Configuration;
using MefContrib.Hosting.Interception.Handlers;
using NUnit.Framework;

namespace MefContrib.Hosting.Interception.Tests.Configuration
{
    [TestFixture]
    public class InterceptionConfigurationTests
    {
        [Test]
        public void Adding_export_handlers_is_reflected_in_the_Handlers_collection()
        {
            var cfg = new InterceptionConfiguration()
                .AddHandler(new GenericExportHandler())
                .AddHandler(new ConcreteTypeExportHandler());
            
            Assert.That(cfg.Handlers.Count(), Is.EqualTo(2));
            Assert.That(cfg.Handlers.OfType<GenericExportHandler>().Any());
            Assert.That(cfg.Handlers.OfType<ConcreteTypeExportHandler>().Any());
        }

        [Test]
        public void Adding_interceptors_is_reflected_in_the_Interceptors_collection()
        {
            var cfg = new InterceptionConfiguration()
                .AddInterceptor(new CompositeValueInterceptor())
                .AddInterceptor(new FakeInterceptor());

            Assert.That(cfg.Interceptors.Count(), Is.EqualTo(2));
            Assert.That(cfg.Interceptors.OfType<CompositeValueInterceptor>().Any());
            Assert.That(cfg.Interceptors.OfType<FakeInterceptor>().Any());
        }

        [Test]
        public void Adding_interception_criteria_is_reflected_in_the_InterceptionCriteria_collection()
        {
            var cfg = new InterceptionConfiguration()
                .AddInterceptionCriteria(new PredicateInterceptionCriteria(new FakeInterceptor(), part => true));

            Assert.That(cfg.InterceptionCriteria.Count(), Is.EqualTo(1));
            Assert.That(cfg.InterceptionCriteria.OfType<PredicateInterceptionCriteria>().Any());
        }

        [Test]
        public void Adding_null_handler_causes_argument_null_exception_to_be_thrown()
        {
            Assert.That(delegate
            {
                new InterceptionConfiguration().AddHandler(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Adding_null_interceptor_causes_argument_null_exception_to_be_thrown()
        {
            Assert.That(delegate
            {
                new InterceptionConfiguration().AddInterceptor(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Adding_null_interception_criteria_causes_argument_null_exception_to_be_thrown()
        {
            Assert.That(delegate
            {
                new InterceptionConfiguration().AddInterceptionCriteria(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }
    }
}