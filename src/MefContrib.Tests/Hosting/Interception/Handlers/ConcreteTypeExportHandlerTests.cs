using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.ReflectionModel;
using System.Linq;
using MefContrib.Hosting.Interception.Handlers;
using MefContrib.Tests;
using NUnit.Framework;

namespace MefContrib.Hosting.Interception.Tests.Handlers
{
    [TestFixture]
    public class ConcreteTypeExportHandlerTests
    {
        public ConcreteTypeExportHandler ConcreteTypeHandler;
        public ImportDefinition RepositoryImportDefinition;

        [Test]
        public void When_querying_using_a_concrete_order_repository_the_order_repository_part_is_created()
        {
            var exports = new List<Tuple<ComposablePartDefinition, ExportDefinition>>();
            var export = ConcreteTypeHandler.GetExports(RepositoryImportDefinition, exports).FirstOrDefault();
            var partType = ReflectionModelServices.GetPartType(export.Item1).Value;
            partType.ShouldBeOfType<CustomerRepository>();
        }

        [TestFixtureSetUp]
        public void TestSetUp()
        {
            ConcreteTypeHandler = new ConcreteTypeExportHandler();
            var typeCatalog = new TypeCatalog(typeof(CustomerProcessor));
            var orderProcessorContract = AttributedModelServices.GetContractName(typeof(CustomerProcessor));
            var orderProcessPartDefinition = typeCatalog.Parts.Single(p => p.ExportDefinitions.Any(d => d.ContractName == orderProcessorContract));
            RepositoryImportDefinition = orderProcessPartDefinition.ImportDefinitions.First();
        }
    }

    [Export]
    public class CustomerRepository
    {
    }

    [Export]
    public class CustomerProcessor
    {
        [Import]
        CustomerRepository CustomerRepository { get; set; }
    }
}
