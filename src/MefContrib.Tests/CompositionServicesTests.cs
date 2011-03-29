using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using MefContrib.Hosting.Generics.Tests;
using NUnit.Framework;

namespace MefContrib.Tests
{
    [TestFixture]
    public class CompositionServicesTests : CompositionServicesContext
    {
        [Test]
        public void When_retrieving_import_definition_type_for_constructor_import_import_1_DummyImport1_is_returned()
        {
            ImportDefinition = DummyPartImports.Single(d => d.ContractName == AttributedModelServices.GetContractName(typeof(IDummyImport1)));
            Assert.AreEqual(typeof(IDummyImport1), ContractServices.GetImportDefinitionType(ImportDefinition));
        }

        [Test]
        public void When_retrieving_import_definition_type_for_property_import_import_2_DummyImport2_is_returned()
        {
            ImportDefinition = DummyPartImports.Single(d => d.ContractName == AttributedModelServices.GetContractName(typeof(IDummyImport2)));
            Assert.AreEqual(typeof(IDummyImport2), ContractServices.GetImportDefinitionType(ImportDefinition));
        }

        [Test]
        public void When_retrieving_import_definition_typoe_for_field_import_import_3_DummyImport3_is_returned()
        {
            ImportDefinition = DummyPartImports.Single(d => d.ContractName == AttributedModelServices.GetContractName(typeof(IDummyImport3)));
            Assert.AreEqual(typeof(IDummyImport3), ContractServices.GetImportDefinitionType(ImportDefinition));
        }

        [Test]
        public void Calling_GetImportDefinitionType_with_null_importDefinition_throws_an_exception()
        {
            Assert.That(delegate
            {
                ContractServices.GetImportDefinitionType(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Calling_IsReflectionImportDefinition_with_null_importDefinition_throws_an_exception()
        {
            Assert.That(delegate
            {
                ContractServices.IsReflectionImportDefinition(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }
    }

    public abstract class CompositionServicesContext
    {
        public CompositionServicesContext()
        {
            var catalog = new TypeCatalog(typeof(DummyPart));
            var part = catalog.Parts.First();
            DummyPartImports = part.ImportDefinitions;
        }

        public IEnumerable<ImportDefinition> DummyPartImports { get; set; }
        public ImportDefinition ImportDefinition { get; set; }
    }

    [Export]
    public class DummyPart
    {
        [ImportingConstructor]
        public DummyPart(IDummyImport1 import1)
        {
        }

        [Import]
        public IDummyImport2 Import2 { get; set; }

        [Import]
        public IDummyImport3 Import3;

        [Import]
        public IRepository<Order> Repository { get; set; }
    }

    public interface IDummyImport1 { }

    public interface IDummyImport2 { }

    public interface IDummyImport3 { }
}