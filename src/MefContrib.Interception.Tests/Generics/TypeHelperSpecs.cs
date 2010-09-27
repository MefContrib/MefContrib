using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using MefContrib.Interception.Generics;
using NUnit.Framework;

namespace MefContrib.Interception.Tests.Generics
{
    namespace Given_using_TypeHelper
    {
        [TestFixture]
        public class when_retrieving_import_definition_type_for_constructor_import_import_1 : TypeHelperContext
        {
            [Test]
            public void DummyImport1_is_returned()
            {
                Assert.AreEqual(typeof(IDummyImport1), TypeHelper.GetImportDefinitionType(ImportDefinition));
            }


            public override void Context()
            {
                ImportDefinition = DummyPartImports.Single(d => d.ContractName == AttributedModelServices.GetContractName(typeof(IDummyImport1)));
            }
        }

        [TestFixture]
        public class when_retrieving_import_definition_type_for_property_import_import_2 : TypeHelperContext
        {
            [Test]
            public void DummyImport2_is_returned()
            {
                Assert.AreEqual(typeof(IDummyImport2), TypeHelper.GetImportDefinitionType(ImportDefinition));
            }


            public override void Context()
            {
                ImportDefinition = DummyPartImports.Single(d => d.ContractName == AttributedModelServices.GetContractName(typeof(IDummyImport2)));
            }
        }

        [TestFixture]
        public class when_retrieving_import_definition_typoe_for_field_import_import_3 : TypeHelperContext
        {
            [Test]
            public void DummyImport3_is_returned()
            {
                Assert.AreEqual(typeof(IDummyImport3), TypeHelper.GetImportDefinitionType(ImportDefinition));
            }


            public override void Context()
            {
                ImportDefinition = DummyPartImports.Single(d => d.ContractName == AttributedModelServices.GetContractName(typeof(IDummyImport3)));
            }

        }

        [TestFixture]
        public class When_building_a_closed_generic_repository : TypeHelperContext
        {
            [Test]
            public void order_repository_is_returned()
            {
                Assert.AreEqual(typeof(Repository<Order>), OrderRepositoryType);
            }

            public override void Context()
            {
                var importDefinitionType = typeof(IRepository<Order>);
                var typeMapping = new Dictionary<Type, Type>();
                typeMapping.Add(typeof(IRepository<>), typeof(Repository<>));
                OrderRepositoryType = TypeHelper.BuildGenericType(importDefinitionType, typeMapping);
            }

            public Type OrderRepositoryType { get; set; }
        }

        public class TypeHelperContext
        {
            public TypeHelperContext()
            {
                var catalog = new TypeCatalog(typeof(DummyPart));
                var part = catalog.Parts.First();
                DummyPartImports = part.ImportDefinitions;

                Context();
            }

            public IEnumerable<ImportDefinition> DummyPartImports { get; set; }
            public ImportDefinition ImportDefinition { get; set; }

            public virtual void Context()
            {
            }
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
