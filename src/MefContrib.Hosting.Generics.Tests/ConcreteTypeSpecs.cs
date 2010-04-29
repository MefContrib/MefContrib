using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Text;
using MefContrib.Hosting.Generics.Tests.GenericTypeSpecs;
using NUnit.Framework;

namespace MefContrib.Hosting.Generics.Tests.ConcreteTypeSpecs
{
    [TestFixture]
    public class When_querying_container_for_an_order_repository_that_is_not_present_passing_a_concrete_type : GenericCatalogContext
    {
        [Test]
        public void order_repository_is_created()
        {
            Assert.IsNotNull(_orderRepository);
        }

        public override void Context()
        {
            var container = new CompositionContainer(_genericCatalog);
            var export = container.GetExports(_repositoryImportDefinition).FirstOrDefault();
            _orderRepository = (OrderRepository) export.Value;
        }

    }

    public class GenericCatalogContext
    {
        protected AggregateCatalog _aggegateCatalog;
        protected GenericCatalog _genericCatalog;
        protected ImportDefinition _repositoryImportDefinition;
        protected OrderRepository _orderRepository;

        public GenericCatalogContext()
        {
            var typeCatalog = new TypeCatalog(typeof(OrderProcessor));
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

    [Export]
    public class OrderRepository
    {
        
    }

    [Export]
    public class OrderProcessor
    {
        [Import]
        OrderRepository OrderRepository { get; set; }
    }


}
