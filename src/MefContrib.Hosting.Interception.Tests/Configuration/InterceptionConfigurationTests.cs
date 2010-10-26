using System.Linq;
using MefContrib.Hosting.Interception.Configuration;
using MefContrib.Hosting.Interception.Handlers;
using NUnit.Framework;

namespace MefContrib.Hosting.Interception.Tests.Configuration
{
    [TestFixture]
    public class InterceptionConfigurationTests
    {
        [Test]
        public void Adding_export_handlers_is_reflecteed_in_the_Handlers_collection()
        {
            var cfg = new InterceptionConfiguration()
                .AddHandler(new GenericExportHandler())
                .AddHandler(new ConcreteTypeExportHandler());
            
            Assert.That(cfg.Handlers.Count(), Is.EqualTo(2));
            Assert.That(cfg.Handlers.OfType<GenericExportHandler>().Any());
            Assert.That(cfg.Handlers.OfType<ConcreteTypeExportHandler>().Any());
        }

        [Test]
        public void Adding_interceptors_is_reflecteed_in_the_Interceptors_collection()
        {
            var cfg = new InterceptionConfiguration()
                .AddInterceptor(new CompositeValueInterceptor())
                .AddInterceptor(new EmptyInterceptor());

            Assert.That(cfg.Interceptors.Count(), Is.EqualTo(2));
            Assert.That(cfg.Interceptors.OfType<CompositeValueInterceptor>().Any());
            Assert.That(cfg.Interceptors.OfType<EmptyInterceptor>().Any());
        }

        [Test]
        public void Adding_s_is_reflecteed_in_the_Interceptors_collection()
        {
            var cfg = new InterceptionConfiguration()
                .AddInterceptionCriteria(new PredicateInterceptionCriteria(new EmptyInterceptor(), part => true));

            Assert.That(cfg.InterceptionCriteria.Count(), Is.EqualTo(1));
            Assert.That(cfg.InterceptionCriteria.OfType<PredicateInterceptionCriteria>().Any());
        }
    }
}