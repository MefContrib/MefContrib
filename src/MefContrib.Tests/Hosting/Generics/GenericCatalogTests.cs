using System.ComponentModel.Composition.Hosting;
using NUnit.Framework;

namespace MefContrib.Hosting.Generics.Tests
{
    [TestFixture]
    public class GenericCatalogTests
    {
        public ExportProvider ExportProvider;

        [TestFixtureSetUp]
        public void TestSetUp()
        {
            var typeCatalog = new TypeCatalog(
                typeof(CtorOrderProcessor),
                typeof(OrderProcessor),
                typeof(ConcreteCtorOrderProcessor),
                typeof(ConcreteOrderProcessor),
                typeof(MyCtorOrderProcessor),
                typeof(MyOrderProcessor),
                typeof(MyOrderProcessorSetterOnly));
            var genericCatalog = new GenericCatalog(new TestGenericContractRegistry());
            var aggregateCatalog = new AggregateCatalog(typeCatalog, genericCatalog);
            var provider = new CatalogExportProvider(aggregateCatalog);
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

        [Test]
        public void When_querying_for_concrete_order_processor_the_order_processor_is_created()
        {
            var orderProcessor = ExportProvider.GetExportedValue<ConcreteOrderProcessor>();
            Assert.That(orderProcessor, Is.Not.Null);
            Assert.That(orderProcessor.OrderRepository, Is.Not.Null);
        }

        [Test]
        public void When_querying_for_concrete_ctor_order_processor_the_ctor_order_processor_is_created()
        {
            var orderProcessor = ExportProvider.GetExportedValue<ConcreteCtorOrderProcessor>();
            Assert.That(orderProcessor, Is.Not.Null);
            Assert.That(orderProcessor.OrderRepository, Is.Not.Null);
        }

        [Test]
        public void When_querying_for_my_order_processor_the_order_processor_is_created()
        {
            var orderProcessor = ExportProvider.GetExportedValue<MyOrderProcessor>();
            Assert.That(orderProcessor, Is.Not.Null);
            Assert.That(orderProcessor.OrderRepository, Is.Not.Null);
        }

        [Test]
        public void When_querying_for_my_order_processor_with_setter_only_the_order_processor_is_created()
        {
            var orderProcessor = ExportProvider.GetExportedValue<MyOrderProcessorSetterOnly>();
            Assert.That(orderProcessor, Is.Not.Null);
            Assert.That(orderProcessor.orderRepository, Is.Not.Null);
        }

        [Test]
        public void When_querying_for_my_ctor_order_processor_the_ctor_order_processor_is_created()
        {
            var orderProcessor = ExportProvider.GetExportedValue<MyCtorOrderProcessor>();
            Assert.That(orderProcessor, Is.Not.Null);
            Assert.That(orderProcessor.OrderRepository, Is.Not.Null);
        }
    }
}