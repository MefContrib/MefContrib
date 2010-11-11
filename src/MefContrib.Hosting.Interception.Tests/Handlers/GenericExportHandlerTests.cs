using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using MefContrib.Hosting.Interception.Handlers;
using MefContrib.Tests;
using NUnit.Framework;

namespace MefContrib.Hosting.Interception.Tests.Handlers
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
                result = GenericExportHandler.GetExports(RepositoryImportDefinition, Enumerable.Empty<Tuple<ComposablePartDefinition, ExportDefinition>>()).Single();
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

        public class GenericExportHandlerContext
        {
            public AggregateCatalog AggegateCatalog;
            public GenericExportHandler GenericExportHandler;
            public ImportDefinition RepositoryImportDefinition;

            public GenericExportHandlerContext()
            {
                var typeCatalog = new TypeCatalog(typeof(OrderProcessor), typeof(RepositoryTypeLocator));
                AggegateCatalog = new AggregateCatalog();
                AggegateCatalog.Catalogs.Add(typeCatalog);
                GenericExportHandler = new GenericExportHandler();
                GenericExportHandler.Initialize(AggegateCatalog);
                var orderProcessorContract = AttributedModelServices.GetContractName(typeof(OrderProcessor));
                var orderProcessPartDefinition = typeCatalog.Parts.Single(p => p.ExportDefinitions.Any(d => d.ContractName == orderProcessorContract));
                RepositoryImportDefinition = orderProcessPartDefinition.ImportDefinitions.First();
                
                Context();
            }

            public virtual void Context()
            {
            }
        }

        public class RepositoryTypeLocator : GenericContractRegistry
        {
            protected override void Initialize()
            {
                Register(typeof(IRepository<>), typeof(Repository<>));
            }
        }

        [Export]
        public class OrderProcessor
        {
            [Import]
            public IRepository<Order> OrderRepository { get; set; }
        }

        public class OrderRepository : IRepository<Order>
        {
        }
    }
}
