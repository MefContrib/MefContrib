using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.ReflectionModel;
using System.Linq;
using System.Text;
using MefContrib.Interception.Generics;
using MefContrib.Tests;
using NUnit.Framework;

namespace MefContrib.Interception.Tests.Generics
{
    namespace Given_a_ConcreteTypeExportHandler
    {
        [TestFixture]
        public class When_querying_using_a_concrete_order_repository : ConcreteTypeExportHandlerContext
        {
            [Test]
            public void order_repository_part_is_created()
            {
                _partType.ShouldBeOfType<OrderRepository>();
            }

            public override void Context()
            {
                var exports = new List<Tuple<ComposablePartDefinition, ExportDefinition>>();
                var export = ConcreteTypeHandler.GetExports(RepositoryImportDefinition, exports).FirstOrDefault();
                _partType = ReflectionModelServices.GetPartType(export.Item1).Value;
            }

            private Type _partType;

        }

        public class ConcreteTypeExportHandlerContext
        {
            public ConcreteTypeExportHandler ConcreteTypeHandler;
            public ImportDefinition RepositoryImportDefinition;
            public OrderRepository OrderRepository;

            public ConcreteTypeExportHandlerContext()
            {
                var typeCatalog = new TypeCatalog(typeof(OrderProcessor));
                var catalog = new AggregateCatalog();
                ConcreteTypeHandler = new ConcreteTypeExportHandler();
                string orderProcessorContract = AttributedModelServices.GetContractName(typeof(OrderProcessor));
                var orderProcessPartDefinition = typeCatalog.Parts.Single(p => p.ExportDefinitions.Any(d => d.ContractName == orderProcessorContract));
                RepositoryImportDefinition = orderProcessPartDefinition.ImportDefinitions.First();
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
}
