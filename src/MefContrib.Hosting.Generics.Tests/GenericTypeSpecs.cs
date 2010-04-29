using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.ReflectionModel;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MefContrib.Hosting.Generics.Tests.GenericCatalogSpecs;
using NUnit.Framework;

namespace MefContrib.Hosting.Generics.Tests.GenericTypeSpecs
{
    [TestFixture]
    public class When_querying_catalog_for_an_order_repository_and_no_closed_repository_is_present : GenericCatalogContext
    {
        [Test]                    
        public void order_repository_part_definition_is_created()
        {
            Assert.IsNotNull(_result.Item1);
        }

        [Test]
        public void order_repository_export_is_created()
        {
            Assert.IsNotNull(_result.Item2);
        }

        public override void Context()
        {
            _result = _genericCatalog.GetExports(_repositoryImportDefinition).Single();
        }

        private Tuple<ComposablePartDefinition, ExportDefinition> _result;
    }

    [TestFixture]
    public class When_querying_catalog_for_an_order_repository_and_closed_repository_is_present : GenericCatalogContext
    {
        [Test]
        public void closed_repository_is_returned()
        {
            Assert.IsTrue(_exportDefinition.Metadata.ContainsKey("Closed"));
        }

        public override void Context()
        {
            _aggegateCatalog.Catalogs.Add(new TypeCatalog(typeof(OrderRepository)));
            var result = _genericCatalog.GetExports(_repositoryImportDefinition).Single();
            _exportDefinition = result.Item2;
        }

        private ExportDefinition _exportDefinition;
    }

    [TestFixture]
    public class When_querying_container_for_an_order_repository_and_no_closed_repository_is_present : GenericCatalogContext
    {
        [Test]
        public void order_repository_is_returned()
        {
            Assert.IsNotNull(_orderRepository);
        }

        public override void Context()
        {
            var container = new CompositionContainer(_genericCatalog);
            var orderRepositoryExport = container.GetExports(_repositoryImportDefinition).Single();
            _orderRepository = (IRepository<Order>) orderRepositoryExport.Value;
        }

        private IRepository<Order> _orderRepository;
    }

    [TestFixture]
    public class When_querying_container_for_an_order_repository_and_closed_repository_is_present : GenericCatalogContext
    {
        [Test]
        public void closed_order_repository_is_returned()
        {
            Assert.IsNotNull(_orderRepository);
        }
        
        public override void Context()
        {
            _aggegateCatalog.Catalogs.Add(new TypeCatalog(typeof(OrderRepository)));
            var container = new CompositionContainer(_genericCatalog);
            var orderRepositoryExport = container.GetExports(_repositoryImportDefinition).Single();
            _orderRepository = (OrderRepository)orderRepositoryExport.Value;
        }

        private OrderRepository  _orderRepository;
    }

    public class GenericCatalogContext
    {
        protected AggregateCatalog _aggegateCatalog;
        protected GenericCatalog _genericCatalog;
        protected ImportDefinition _repositoryImportDefinition;

        public GenericCatalogContext()
        {
            var typeCatalog = new TypeCatalog(typeof(OrderProcessor), typeof(RepositoryTypeLocator));
            _aggegateCatalog = new AggregateCatalog();
            _aggegateCatalog.Catalogs.Add(typeCatalog);
            _genericCatalog = new GenericCatalog(_aggegateCatalog);
            string orderProcessorContract = AttributedModelServices.GetContractName(typeof(OrderProcessor));
            var orderProcessPartDefinition = typeCatalog.Parts.Single(p => p.ExportDefinitions.Any(d => d.ContractName == orderProcessorContract));
            _repositoryImportDefinition = orderProcessPartDefinition.ImportDefinitions.First();
            Context();
        }

        public virtual void Context()
        {
            
        }
    }

    public class RepositoryTypeLocator : GenericContractTypeMapping
    {
        public RepositoryTypeLocator()
            :base(typeof(IRepository<>), typeof(Repository<>))
        {
        }
    }

    [Export]
    public class OrderProcessor
    {
        [Import]
        public IRepository<Order> OrderRepository { get; set; }
    }

    [InheritedExport(typeof(IRepository<Order>))]
    [ExportMetadata("Closed", true)]
    public class OrderRepository : IRepository<Order>
    {
        
    }


}