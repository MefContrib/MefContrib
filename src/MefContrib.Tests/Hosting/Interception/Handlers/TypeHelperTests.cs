using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using MefContrib.Hosting.Generics.Tests;
using MefContrib.Hosting.Interception.Handlers;
using NUnit.Framework;

namespace MefContrib.Hosting.Interception.Tests.Handlers
{
    namespace TypeHelperTests
    {
        [TestFixture]
        public class When_retrieving_import_definition_type_for_constructor_import_import_1 : TypeHelperContext
        {
            [Test]
            public void DummyImport1_is_returned()
            {
                ImportDefinition = DummyPartImports.Single(d => d.ContractName == AttributedModelServices.GetContractName(typeof(IDummyImport1)));
                Assert.AreEqual(typeof(IDummyImport1), TypeHelper.GetImportDefinitionType(ImportDefinition));
            }
        }

        [TestFixture]
        public class When_retrieving_import_definition_type_for_property_import_import_2 : TypeHelperContext
        {
            [Test]
            public void DummyImport2_is_returned()
            {
                ImportDefinition = DummyPartImports.Single(d => d.ContractName == AttributedModelServices.GetContractName(typeof(IDummyImport2)));
                Assert.AreEqual(typeof(IDummyImport2), TypeHelper.GetImportDefinitionType(ImportDefinition));
            }
        }

        [TestFixture]
        public class When_retrieving_import_definition_typoe_for_field_import_import_3 : TypeHelperContext
        {
            [Test]
            public void DummyImport3_is_returned()
            {
                ImportDefinition = DummyPartImports.Single(d => d.ContractName == AttributedModelServices.GetContractName(typeof(IDummyImport3)));
                Assert.AreEqual(typeof(IDummyImport3), TypeHelper.GetImportDefinitionType(ImportDefinition));
            }
        }

        [TestFixture]
        public class When_building_a_closed_generic_repository : TypeHelperContext
        {
            [Test]
            public void Order_repository_is_returned()
            {
                var importDefinitionType = typeof(IRepository<Order>);
                var typeMapping = new Dictionary<Type, Type>
                                      {
                                          {typeof (IRepository<>), typeof (Repository<>)}
                                      };
                var orderRepositoryType = TypeHelper.BuildGenericType(importDefinitionType, typeMapping);

                Assert.AreEqual(typeof(Repository<Order>), orderRepositoryType);
            }
        }

        public abstract class TypeHelperContext
        {
            public TypeHelperContext()
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
}
