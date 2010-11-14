using System.ComponentModel.Composition.Hosting;
using MefContrib.Hosting.Interception.Configuration;
using MefContrib.Hosting.Interception.Handlers;
using NUnit.Framework;

namespace MefContrib.Hosting.Interception.Tests.Handlers
{
    [TestFixture]
    public class GenericExportHandlerIntegrationTests
    {
        public ExportProvider ExportProvider;

        [TestFixtureSetUp]
        public void TestSetUp()
        {
            var typeCatalog = new TypeCatalog(typeof(CtorOrderProcessor), typeof(OrderProcessor), typeof(RepositoryTypeLocator));
            var cfg = new InterceptionConfiguration().AddHandler(new GenericExportHandler());
            var catalog = new InterceptingCatalog(typeCatalog, cfg);

            var provider = new CatalogExportProvider(catalog);
            provider.SourceProvider = provider;

            ExportProvider = provider;
        }

        [Test]
        public void When_querying_for_order_processor_the_order_processor_is_created()
        {
            var orderProcessor = ExportProvider.GetExportedValue<OrderProcessor>();
            Assert.That(orderProcessor, Is.Not.Null);
            Assert.That(orderProcessor.OrderRepository, Is.Not.Null);
        }

        [Test]
        public void When_querying_for_ctor_order_processor_the_ctor_order_processor_is_created()
        {
            var orderProcessor = ExportProvider.GetExportedValue<CtorOrderProcessor>();
            Assert.That(orderProcessor, Is.Not.Null);
            Assert.That(orderProcessor.OrderRepository, Is.Not.Null);
        }
    }
}