using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using MefContrib.Tests;
using NUnit.Framework;

namespace MefContrib.Hosting.Generics.Tests
{
    namespace GenericExportHandlerTests
    {
        [TestFixture]
        public class When_querying_for_an_order_repository_and_no_closed_repository_is_present : GenericExportHandlerContext
        {
            [Test]
            public void Order_repository_part_definition_is_created()
            {
                Assert.IsNotNull(result.Item1);
            }

            [Test]
            public void Order_repository_export_is_created()
            {
                Assert.IsNotNull(result.Item2);
            }

            public override void Context()
            {
                result = GenericExportHandler.GetExports(
                    RepositoryImportDefinition,
                    Enumerable.Empty<Tuple<ComposablePartDefinition, ExportDefinition>>()).Single();
            }

            private Tuple<ComposablePartDefinition, ExportDefinition> result;
        }

        [TestFixture]
        public class When_querying_for_an_order_repository_and_closed_repository_is_passed_in : GenericExportHandlerContext
        {
            [Test]
            public void Closed_generic_repository_is_not_created()
            {
                result.Count().ShouldEqual(1);
            }

            public override void Context()
            {
                var catalog = new TypeCatalog(typeof (OrderRepository));
                var exports = catalog.GetExports(RepositoryImportDefinition);
                result = GenericExportHandler.GetExports(RepositoryImportDefinition, exports);
            }

            private IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> result;
        }

        [TestFixture]
        public class When_querying_for_order_processor : GenericExportHandlerContext
        {
            [Test]
            public void Order_processor_part_is_returned()
            {
                Assert.NotNull(result.Item1);
                Assert.NotNull(result.Item2);
            }

            public override void Context()
            {
                var catalog = new TypeCatalog(typeof(OrderProcessor));
                var exports = catalog.GetExports(OrderProcessorImportDefinition);
                result = GenericExportHandler.GetExports(OrderProcessorImportDefinition, exports).Single();
            }

            private Tuple<ComposablePartDefinition, ExportDefinition> result;
        }

        [TestFixture]
        public class When_querying_for_ctor_order_processor : GenericExportHandlerContext
        {
            [Test]
            public void Ctor_order_processor_part_is_returned()
            {
                Assert.NotNull(result.Item1);
                Assert.NotNull(result.Item2);
            }

            public override void Context()
            {
                var catalog = new TypeCatalog(typeof(CtorOrderProcessor));
                var exports = catalog.GetExports(CtorOrderProcessorImportDefinition);
                result = GenericExportHandler.GetExports(CtorOrderProcessorImportDefinition, exports).Single();
            }

            private Tuple<ComposablePartDefinition, ExportDefinition> result;
        }

        public class GenericExportHandlerContext
        {
            public readonly GenericExportHandler GenericExportHandler;
            public readonly ImportDefinition RepositoryImportDefinition;
            public readonly ImportDefinition OrderProcessorImportDefinition;
            public readonly ImportDefinition CtorOrderProcessorImportDefinition;

            public GenericExportHandlerContext()
            {
                var typeCatalog = new TypeCatalog(typeof(OrderProcessor), typeof(CtorOrderProcessor), typeof(ImportDefinitionHelper), typeof(TestGenericContractRegistry));
                GenericExportHandler = new GenericExportHandler();
                GenericExportHandler.Initialize(typeCatalog);
                
                var orderProcessorContract = AttributedModelServices.GetContractName(typeof(OrderProcessor));
                var orderProcessPartDefinition = typeCatalog.Parts.Single(p => p.ExportDefinitions.Any(d => d.ContractName == orderProcessorContract));
                RepositoryImportDefinition = orderProcessPartDefinition.ImportDefinitions.First();

                var importDefinitionHelperContract = AttributedModelServices.GetContractName(typeof(ImportDefinitionHelper));
                var importDefinitionHelperPartDefinition = typeCatalog.Parts.Single(p => p.ExportDefinitions.Any(d => d.ContractName == importDefinitionHelperContract));

                OrderProcessorImportDefinition = importDefinitionHelperPartDefinition.ImportDefinitions.First();
                CtorOrderProcessorImportDefinition = importDefinitionHelperPartDefinition.ImportDefinitions.Skip(1).First();

                Context();
            }

            public virtual void Context()
            {
            }
        }
    }

    [Export(typeof(IGenericContractRegistry))]
    public class TestGenericContractRegistry : GenericContractRegistryBase
    {
        protected override void Initialize()
        {
            Register(typeof(IRepository<>), typeof(Repository<>));
        }
    }

    [Export]
    public class ImportDefinitionHelper
    {
        [Import]
        public OrderProcessor OrderProcessor { get; set; }

        [Import]
        public CtorOrderProcessor CtorOrderProcessor { get; set; }
    }

    [Export]
    public class OrderProcessor
    {
        [Import]
        public IRepository<Order> OrderRepository { get; set; }
    }

    [Export]
    public class CtorOrderProcessor
    {
        [ImportingConstructor]
        public CtorOrderProcessor(IRepository<Order> orderRepository)
        {
            OrderRepository = orderRepository;
        }

        public IRepository<Order> OrderRepository { get; set; }
    }

    public class OrderRepository : IRepository<Order>
    {
    }
}
