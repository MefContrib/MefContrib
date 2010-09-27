using MefContrib.Tests;

namespace MefContrib.Hosting.Conventions.Tests
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;
    using System.ComponentModel.Composition.ReflectionModel;
    using System.Linq;
    using System.Reflection;
    using MefContrib.Hosting.Conventions.Configuration;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class ConventionCatalogTests
    {
        private static ConventionCatalog CreateConventionCatalog(IEnumerable<IPartConvention> conventions, ITypeLoader typeLoader)
        {
            return new ConventionCatalog(conventions, typeLoader);
        }

        private static ConventionCatalog CreateDefaultConventionCatalog()
        {
            var registry =
                new FakeConventionRegistry();

            return CreateConventionCatalog(registry.GetConventions(), CreateDefaultTypeLoader());
        }

        private static ConventionCatalog CreateDefaultConventionCatalogWithTypeLoader(ITypeLoader typeLoader)
        {
            var registry =
                new FakeConventionRegistry();

            return CreateConventionCatalog(registry.GetConventions(), typeLoader);
        }

        private static ConventionCatalog CreateDefaultConventionCatalogWithEmptyTypeLoader()
        {
            var registry =
                new FakeConventionRegistry();

            return CreateConventionCatalog(registry.GetConventions(), CreateEmptyTypeLoader());
        }

        private static ConventionCatalog CreateDefaultConventionCatalogWithEmptyConventions()
        {
            var registry =
                new FakeConventionRegistry();

            return CreateConventionCatalog(new List<IPartConvention>(), CreateEmptyTypeLoader());
        }

        private static ITypeLoader CreateDefaultTypeLoader()
        {
            var typeLoader =
                new TypeLoader();
            typeLoader.AddTypes(() => new[] { typeof(FakePart) });

            return typeLoader;
        }

        private static Mock<ITypeLoader> CreateMockTypeLoader()
        {
            var typeLoader =
                new Mock<ITypeLoader>();
            typeLoader.Setup(t => t.GetTypes(p => true)).Returns(new List<Type> { typeof(FakePart) });

            return typeLoader;
        }

        private static ITypeLoader CreateEmptyTypeLoader()
        {
            return new TypeLoader();
        }

        [Test]
        public void Ctor_should_throw_argument_null_exception_when_called_with_null_registry_collection_and_type_loader_instance()
        {
            var exception =
                Catch.Exception(() => new ConventionCatalog((IEnumerable<IConventionRegistry<IPartConvention>>)null, new TypeLoader()));

            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Test]
        public void Ctor_should_throw_argument_null_exception_when_called_with_registry_collection_and_null_type_loader()
        {
            var exception =
                Catch.Exception(() => new ConventionCatalog(new[] { new FakeConventionRegistry() }, null));

            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Test]
        public void Ctor_should_throw_argument_null_exception_when_called_with_null_convention_collection_and_type_loader_instance()
        {
            var exception =
                Catch.Exception(() => new ConventionCatalog((IEnumerable<IPartConvention>)null, new TypeLoader()));

            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Test]
        public void Ctor_should_throw_argument_null_exception_when_called_with_conventions_collection_and_null_type_loader()
        {
            var exception =
                Catch.Exception(() => new ConventionCatalog(new[] { new FakePartConvention() }, null));

            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Test]
        public void Ctor_should_retrieve_conventions_from_all_registries_when_called_with_convention_registry_collection()
        {
            var registries =
                new List<Mock<IConventionRegistry<IPartConvention>>>
                {
                    new Mock<IConventionRegistry<IPartConvention>>(),
                    new Mock<IConventionRegistry<IPartConvention>>()
                };

            var catalog =
                new ConventionCatalog(registries.Select(x => x.Object), new TypeLoader());

            registries.ForEach(x => x.VerifyAll());
        }

        [Test]
        public void Ctor_should_set_conventions_when_called_with_convention_registry_collection()
        {
            var registry =
                new Mock<IConventionRegistry<IPartConvention>>();
            
            registry.Setup(x => x.GetConventions()).Returns(new[] { new FakePartConvention() });

            var catalog =
                new ConventionCatalog(new[] { registry.Object }, new TypeLoader());

            catalog.Conventions.Count().ShouldEqual(1);
        }

        [Test]
        public void Ctor_should_set_type_loader_when_called_with_convention_registry_collection()
        {
            var registry =
               new Mock<IConventionRegistry<IPartConvention>>();

            registry.Setup(x => x.GetConventions()).Returns(new[] { new FakePartConvention() });

            var catalog =
                new ConventionCatalog(new[] { registry.Object }, new TypeLoader());

            catalog.TypeLoader.ShouldNotBeNull();
        }

        [Test]
        public void Ctor_should_set_conventions_when_called_with_conventions_collection()
        {
            var conventions =
                new[] { new FakePartConvention() };

            var catalog =
                new ConventionCatalog(conventions, new TypeLoader());

            catalog.Conventions.Count().ShouldEqual(1);
        }

        [Test]
        public void Ctor_should_set_type_loader_when_called_with_conventions_collection()
        {
            var conventions =
                new[] { new FakePartConvention() };

            var catalog =
                new ConventionCatalog(conventions, new TypeLoader());

            catalog.TypeLoader.ShouldNotBeNull();
        }

        [Test]
        public void Parts_should_return_empty_enumerable_when_type_loader_is_empty()
        {
            var catalog =
                CreateDefaultConventionCatalogWithEmptyTypeLoader();

            catalog.Parts.Count().ShouldEqual(0);
        }

        [Test]
        public void Parts_should_return_empty_enumerable_when_no_conventions_have_been_defined()
        {
            var catalog =
                CreateDefaultConventionCatalogWithEmptyConventions();

            catalog.Parts.Count().ShouldEqual(0);
        }

        [Test]
        public void Parts_should_retrieve_types_from_type_loader_once_per_convention()
        {
            var typeLoader =
                CreateMockTypeLoader();

            var conventions =
                new List<IPartConvention>
                {
                    new PartConvention { Condition = x => true },
                    new PartConvention { Condition = x => false }
                };

            var catalog =
                CreateConventionCatalog(conventions, typeLoader.Object);

            var parts =
                catalog.Parts;

            typeLoader.Verify(t => t.GetTypes(It.IsAny<Predicate<Type>>()), Times.Exactly(catalog.Conventions.Count()));
        }

        [Test]
        public void Parts_should_return_defintion_per_type_matched_by_condition_set_on_the_conventions()
        {
            var conventions =
                new List<IPartConvention>
                {
                    new PartConvention { Condition = x => true },
                    new PartConvention { Condition = x => false }
                };

            var catalog =
                CreateConventionCatalog(conventions, CreateDefaultTypeLoader());

            catalog.Parts.Count().ShouldEqual(1);
        }

        [Test]
        public void Parts_should_return_definition_with_correct_type()
        {
            var conventions =
                new List<IPartConvention>
                {
                    new PartConvention { Condition = x => x.Equals(typeof(FakePart)) },
                };

            var catalog =
                CreateConventionCatalog(conventions, CreateDefaultTypeLoader());

            var partType =
                ReflectionModelServices.GetPartType(catalog.Parts.First());

            partType.Value.Equals(typeof(FakePart));
        }

        [Test]
        public void Parts_should_add_creation_policy_metadata_to_part_definition_from_convention()
        {
            var catalog =
                CreateDefaultConventionCatalog();

            var part =
                catalog.Parts.First();

            part.Metadata.ContainsKey(CompositionConstants.PartCreationPolicyMetadataName).ShouldBeTrue();
        }

        [Test]
        public void Parts_should_add_metadata_to_part_definition_from_convention()
        {
            var catalog =
                CreateDefaultConventionCatalog();

            var convention =
                catalog.Conventions.First();

            var part =
                catalog.Parts.First();

            var expectedMetadata =
                new Dictionary<string, object>
                {
                    { CompositionConstants.PartCreationPolicyMetadataName, convention.CreationPolicy },
                    { "Foo", "Bar" }
                };

            expectedMetadata.All(x => part.Metadata.ContainsKey(x.Key)).ShouldBeTrue();
        }

        [Test]
        public void Parts_should_return_definition_with_the_same_number_of_exports_definitions_as_members_defined_on_export_convention()
        {
            var catalog =
                CreateDefaultConventionCatalog();

            var part =
                catalog.Parts.First();

            part.ExportDefinitions.Count().ShouldEqual(2);
        }

        [Test]
        public void Parts_should_set_contract_name_on_export_definition_to_contract_name_from_export_convention_by_extracting_from_member_info()
        {
            var catalog =
                CreateDefaultConventionCatalog();

            var exportConvention =
                catalog.Conventions.First().Exports.First();

            var member =
                ReflectionServices.GetField<FakePart>(x => x.Count);

            exportConvention.ContractName = x => x.Name;
            exportConvention.Members = x => new[] { member };

            var export =
                catalog.Parts.First().ExportDefinitions.First();

            export.ContractName.ShouldEqual(exportConvention.ContractName.Invoke(member));
        }

        [Test]
        public void Parts_should_set_contract_name_on_export_definition_to_contract_name_from_export_convention()
        {
            var catalog =
                CreateDefaultConventionCatalog();

            var exportConvention =
                catalog.Conventions.First().Exports.First();

            var export =
                catalog.Parts.First().ExportDefinitions.First();

            export.ContractName.ShouldEqual(exportConvention.ContractName.Invoke(null));
        }

        [Test]
        public void Parts_should_set_contract_name_on_export_definition_to_contract_type_from_export_convention()
        {
            var catalog =
                CreateDefaultConventionCatalog();

            var exportConvention =
                catalog.Conventions.First().Exports.First();

            exportConvention.ContractName = null;

            var export =
                catalog.Parts.First().ExportDefinitions.First();

            var expectedContractName =
                AttributedModelServices.GetContractName(exportConvention.ContractType.Invoke(null));

            export.ContractName.ShouldEqual(expectedContractName);
        }

        [Test]
        public void Parts_should_set_contract_name_on_export_definition_to_method_type_identity_from_export_convention()
        {
            var catalog =
                CreateDefaultConventionCatalog();

            var exportConvention =
                catalog.Conventions.First().Exports.First();

            var member =
                ReflectionServices.GetMethod<FakePart>(x => x.DoWork());

            exportConvention.ContractName = null;
            exportConvention.ContractType = null;
            exportConvention.Members = x => new[] { member };

            var export =
                catalog.Parts.First().ExportDefinitions.First();

            var expectedContractName =
                AttributedModelServices.GetTypeIdentity((MethodInfo)member);

            export.ContractName.ShouldEqual(expectedContractName);
        }

        [Test]
        public void Parts_should_set_contract_name_on_export_definition_to_declaring_type_of_field_member_from_export_convention()
        {
            var catalog = 
                CreateDefaultConventionCatalog();

            var exportConvention =
                catalog.Conventions.First().Exports.First();

            var member =
                ReflectionServices.GetField<FakePart>(x => x.Count);

            exportConvention.ContractName = null;
            exportConvention.ContractType = null;
            exportConvention.Members = x => new[] { member };

            var export =
                catalog.Parts.First().ExportDefinitions.First();

            var expectedContractName =
                AttributedModelServices.GetContractName(member.GetContractMember());

            export.ContractName.ShouldEqual(expectedContractName);
        }

        [Test]
        public void Parts_should_set_contract_name_on_export_definition_to_declaring_type_of_property_member_from_export_convention()
        {
            var catalog =
                CreateDefaultConventionCatalog();

            var exportConvention =
                catalog.Conventions.First().Exports.First();

            var member =
                ReflectionServices.GetProperty<FakePart>(x => x.Name);

            exportConvention.ContractName = null;
            exportConvention.ContractType = null;
            exportConvention.Members = x => new[] { member };

            var export =
                catalog.Parts.First().ExportDefinitions.First();

            var expectedContractName =
                AttributedModelServices.GetContractName(member.GetContractMember());

            export.ContractName.ShouldEqual(expectedContractName);
        }

        [Test]
        public void Parts_should_set_contract_name_on_export_definition_to_declaring_type_of_type_member_from_export_convention()
        {
            var catalog =
                CreateDefaultConventionCatalog();

            var exportConvention =
                catalog.Conventions.First().Exports.First();

            var member =
                typeof(FakePart);

            exportConvention.ContractName = null;
            exportConvention.ContractType = null;
            exportConvention.Members = x => new[] { member };

            var export =
                catalog.Parts.First().ExportDefinitions.First();

            var expectedContractName =
                AttributedModelServices.GetContractName(member.GetContractMember());

            export.ContractName.ShouldEqual(expectedContractName);
        }

        [Theory]
        [TestCase("Foo", "Bar")]
        [TestCase("Name", "ValueType")]
        public void Parts_should_set_metadata_on_export_definition_from_export_convention(string key, object value)
        {
            var catalog =
                CreateDefaultConventionCatalog();

            var exportConvention =
                catalog.Conventions.First().Exports.First();

            exportConvention.Metadata.Add(new MetadataItem(key, value));

            var part =
                catalog.Parts.First();

            var expectedMetadata =
                new Dictionary<string, object>
                {
                    { key, value }
                };

            var export =
                part.ExportDefinitions.First();

            expectedMetadata.All(x => export.Metadata.ContainsKey(x.Key)).ShouldBeTrue();
        }

        [Test]
        public void Parts_should_add_type_identity_metadata_to_export_definition_from_export_convention_by_extracting_from_member_info()
        {
            var catalog =
                CreateDefaultConventionCatalog();

            var exportConvention =
                catalog.Conventions.First().Exports.First();

            var member =
                ReflectionServices.GetField<FakePart>(x => x.Count);

            exportConvention.ContractType = x => x.DeclaringType;
            exportConvention.Members = x => new[] { member };

            var expectedMetadataItem =
                new KeyValuePair<string, object>(CompositionConstants.ExportTypeIdentityMetadataName,
                    member.DeclaringType.ToString());

            var export =
                catalog.Parts.First().ExportDefinitions.First();

            export.Metadata.Contains(expectedMetadataItem).ShouldBeTrue();
        }

        [Test]
        public void Parts_should_add_type_identity_metadata_to_export_definition_from_export_convention()
        {
            var catalog =
                CreateDefaultConventionCatalog();

            var exportConvention =
                catalog.Conventions.First().Exports.First();

            var export =
                catalog.Parts.First().ExportDefinitions.First();

            var expectedMetadataItem =
                new KeyValuePair<string, object>(CompositionConstants.ExportTypeIdentityMetadataName,
                    AttributedModelServices.GetTypeIdentity(exportConvention.ContractType.Invoke(null)));

            export.Metadata.Contains(expectedMetadataItem).ShouldBeTrue();
        }

        [Test]
        public void Parts_should_add_default_type_identity_metadata_for_type_export()
        {
            var catalog =
                CreateDefaultConventionCatalog();

            var exportConvention =
                catalog.Conventions.First().Exports.First();

            var member =
                typeof(FakePart);

            exportConvention.ContractName = null;
            exportConvention.ContractType = null;
            exportConvention.Members = x => new[] { member };

            var export =
                catalog.Parts.First().ExportDefinitions.First();

            var expectedMetadataItem =
                new KeyValuePair<string, object>(CompositionConstants.ExportTypeIdentityMetadataName,
                    AttributedModelServices.GetTypeIdentity(member.GetContractMember()));

            export.Metadata.Contains(expectedMetadataItem).ShouldBeTrue();
        }

        [Test]
        public void Parts_should_add_default_field_identity_metadata_for_type_export()
        {
            var catalog =
                CreateDefaultConventionCatalog();

            var exportConvention =
                catalog.Conventions.First().Exports.First();

            var member =
                ReflectionServices.GetField<FakePart>(x => x.Count);

            exportConvention.ContractName = null;
            exportConvention.ContractType = null;
            exportConvention.Members = x => new[] { member };

            var export =
                catalog.Parts.First().ExportDefinitions.First();

            var expectedMetadataItem =
                new KeyValuePair<string, object>(CompositionConstants.ExportTypeIdentityMetadataName,
                    AttributedModelServices.GetTypeIdentity(member.GetContractMember()));

            export.Metadata.Contains(expectedMetadataItem).ShouldBeTrue();
        }

        [Test]
        public void Parts_should_add_default_property_identity_metadata_for_type_export()
        {
            var catalog =
                CreateDefaultConventionCatalog();

            var exportConvention =
                catalog.Conventions.First().Exports.First();

            var member =
                ReflectionServices.GetProperty<FakePart>(x => x.Name); ;

            exportConvention.ContractName = null;
            exportConvention.ContractType = null;
            exportConvention.Members = x => new[] { member };

            var export =
                catalog.Parts.First().ExportDefinitions.First();

            var expectedMetadataItem =
                new KeyValuePair<string, object>(CompositionConstants.ExportTypeIdentityMetadataName,
                    AttributedModelServices.GetTypeIdentity(member.GetContractMember()));

            export.Metadata.Contains(expectedMetadataItem).ShouldBeTrue();
        }

        [Test]
        public void Parts_should_add_default_method_identity_metadata_for_type_export()
        {
            var catalog =
                CreateDefaultConventionCatalog();

            var exportConvention =
                catalog.Conventions.First().Exports.First();

            var member =
                ReflectionServices.GetMethod<FakePart>(x => x.DoWork());

            exportConvention.ContractName = null;
            exportConvention.ContractType = null;
            exportConvention.Members = x => new[] { member };

            var export =
                catalog.Parts.First().ExportDefinitions.First();

            var expectedMetadataItem =
                new KeyValuePair<string, object>(CompositionConstants.ExportTypeIdentityMetadataName,
                    AttributedModelServices.GetTypeIdentity((MethodInfo)member));

            export.Metadata.Contains(expectedMetadataItem).ShouldBeTrue();
        }

        [Test]
        public void Parts_should_return_import_definition_with_the_same_number_of_import_definitions_as_defined_on_import_convention()
        {
            var catalog =
                CreateDefaultConventionCatalog();

            var part =
                catalog.Parts.First();

            part.ImportDefinitions.Count().ShouldEqual(2);
        }

        [Test]
        public void Parts_should_set_cardinality_on_import_defintion_to_exaclyone_when_import_convention_not_is_for_collection_and_default_values_are_not_allowed()
        {
            var catalog =
                CreateDefaultConventionCatalog();

            var importConvention =
                catalog.Conventions.First().Imports.First();

            importConvention.AllowDefaultValue = false;

            var part =
                catalog.Parts.First();

            var import =
                part.ImportDefinitions.Cast<ContractBasedImportDefinition>().First();

            import.Cardinality.ShouldEqual(ImportCardinality.ExactlyOne);
        }

        [Test]
        public void Parts_should_set_cardinality_on_import_defintion_to_zeroorone_when_import_convention_not_is_for_collection_and_default_values_are_allowed()
        {
            var catalog =
                CreateDefaultConventionCatalog();

            var importConvention =
                catalog.Conventions.First().Imports.First();

            importConvention.AllowDefaultValue = true;

            var part =
                catalog.Parts.First();

            var import =
                part.ImportDefinitions.Cast<ContractBasedImportDefinition>().First();

            import.Cardinality.ShouldEqual(ImportCardinality.ZeroOrOne);
        }

        [Theory]
        [TestCase(false)]
        [TestCase(true)]
        public void Parts_should_set_cardinality_on_import_definition_to_zeroormore_when_import_convention_is_for_collection(
            bool allowDefaultValues)
        {
            var catalog =
                CreateDefaultConventionCatalog();

            var importConvention =
                catalog.Conventions.First().Imports.First();

            var member =
                ReflectionServices.GetProperty<FakePart>(t => t.Values);

            importConvention.AllowDefaultValue = allowDefaultValues;
            importConvention.Members = x => new[] { member };

            var part =
                catalog.Parts.First();

            var import =
                part.ImportDefinitions.Cast<ContractBasedImportDefinition>().First();

            import.Cardinality.ShouldEqual(ImportCardinality.ZeroOrMore);
        }

        [Test]
        public void Parts_should_set_contract_name_on_import_definition_to_contract_name_from_import_convention_by_extracting_from_member_info()
        {
            var catalog =
                CreateDefaultConventionCatalog();

            var importConvention =
                catalog.Conventions.First().Imports.First();

            var member =
                ReflectionServices.GetField<FakePart>(x => x.Count);

            importConvention.ContractName = x => x.Name;
            importConvention.Members = x => new[] { member };

            var import =
                catalog.Parts.First().ImportDefinitions.First();

            import.ContractName.ShouldEqual(importConvention.ContractName.Invoke(member));
        }

        [Test]
        public void Parts_should_set_contract_name_on_import_definition_to_contract_name_from_import_convention_when_not_null()
        {
            var catalog =
                CreateDefaultConventionCatalog();

            var importConvention =
                catalog.Conventions.First().Imports.First();

            var part =
                catalog.Parts.First();

            var import =
                part.ImportDefinitions.Cast<ContractBasedImportDefinition>().First();

            import.ContractName.ShouldEqual(importConvention.ContractName.Invoke(null));
        }

        [Test]
        public void Parts_should_set_contract_name_on_import_definition_to_contract_type_from_import_convention_when_contract_name_is_null()
        {
            var catalog =
                CreateDefaultConventionCatalog();

            var importConvention =
                catalog.Conventions.First().Imports.First();

            importConvention.ContractName = null;

            var part =
                catalog.Parts.First();

            var import =
                part.ImportDefinitions.Cast<ContractBasedImportDefinition>().First();

            var expectedContractName =
                AttributedModelServices.GetContractName(importConvention.ContractType.Invoke(null));

            import.ContractName.ShouldEqual(expectedContractName);
        }

        [Test]
        public void Parts_should_set_contract_name_on_import_definition_to_declaring_type_from_import_convention_when_contract_name_and_contract_type_is_null_and_member_is_a_field()
        {
            var catalog =
                CreateDefaultConventionCatalog();

            var importConvention =
                catalog.Conventions.First().Imports.First();

            var member =
                ReflectionServices.GetField<FakePart>(x => x.Count);

            importConvention.ContractName = null;
            importConvention.ContractType = null;
            importConvention.Members = x => new[] { member };

            var part =
                catalog.Parts.First();

            var import =
                part.ImportDefinitions.Cast<ContractBasedImportDefinition>().First();

            var expectedContractName =
                AttributedModelServices.GetContractName(member.GetContractMember());

            import.ContractName.ShouldEqual(expectedContractName);
        }

        [Test]
        public void Parts_should_set_contract_name_on_import_definition_to_declaring_type_from_import_convention_when_contract_name_and_contract_type_is_null_and_member_is_a_property()
        {
            var catalog =
               CreateDefaultConventionCatalog();

            var importConvention =
                catalog.Conventions.First().Imports.First();

            var member =
                ReflectionServices.GetProperty<FakePart>(x => x.Name);

            importConvention.ContractName = null;
            importConvention.ContractType = null;
            importConvention.Members = x => new[] { member };

            var part =
                catalog.Parts.First();

            var import =
                part.ImportDefinitions.Cast<ContractBasedImportDefinition>().First();

            var expectedContractName =
                AttributedModelServices.GetContractName(member.GetContractMember());

            import.ContractName.ShouldEqual(expectedContractName);
        }

        [Theory]
        [TestCase(CreationPolicy.Any)]
        [TestCase(CreationPolicy.NonShared)]
        [TestCase(CreationPolicy.Shared)]
        public void Parts_should_set_creation_policy_on_import_definition_from_the_import_convention(CreationPolicy policy)
        {
            var catalog =
               CreateDefaultConventionCatalog();

            var importConvention =
                catalog.Conventions.First().Imports.First();

            importConvention.CreationPolicy = policy;

            var part =
                catalog.Parts.First();

            var import =
                part.ImportDefinitions.Cast<ContractBasedImportDefinition>().First();

            import.RequiredCreationPolicy.ShouldEqual(importConvention.CreationPolicy);
        }

        [Theory]
        [TestCase(true)]
        [TestCase(false)]
        public void Parts_should_set_isrecomposable_on_import_definition_from_import_convention(bool recomposable)
        {
            var catalog =
                CreateDefaultConventionCatalog();

            var importConvention =
                catalog.Conventions.First().Imports.First();

            importConvention.Recomposable = recomposable;

            var part =
                catalog.Parts.First();

            var import =
                part.ImportDefinitions.Cast<ContractBasedImportDefinition>().First();

            import.IsRecomposable.ShouldEqual(importConvention.Recomposable);
        }

        [Test]
        public void Parts_should_set_method_type_identity_on_import_definition_when_contract_type_on_import_convention_is_a_delegate()
        {
            var catalog =
                CreateDefaultConventionCatalog();

            var importConvention =
                catalog.Conventions.First().Imports.First();

            var member =
                ReflectionServices.GetProperty<FakePart>(x => x.Delegate);

            importConvention.Members = x => new[] { member };

            var part =
                catalog.Parts.First();

            var import =
                part.ImportDefinitions.Cast<ContractBasedImportDefinition>().First();

            var expectedTypeIdentity =
                AttributedModelServices.GetTypeIdentity(member.PropertyType.GetMethod("Invoke"));

            import.RequiredTypeIdentity.ShouldEqual(expectedTypeIdentity);
        }

        [Test]
        public void Parts_should_set_type_identity_on_import_definition_to_contract_type_from_import_convention_by_extracting_from_member_info()
        {
            var catalog =
                CreateDefaultConventionCatalog();

            var importConvention =
                catalog.Conventions.First().Imports.First();

            var member =
                ReflectionServices.GetField<FakePart>(x => x.Count);

            importConvention.ContractType = x => x.DeclaringType;
            importConvention.Members = x => new[] { member };

            var part =
                catalog.Parts.First();

            var import =
                part.ImportDefinitions.Cast<ContractBasedImportDefinition>().First();

            import.RequiredTypeIdentity.ShouldEqual(member.DeclaringType.ToString());
        }

        [Test]
        public void Parts_should_set_type_identity_on_import_definition_to_null_when_contact_type_on_import_convention_is_object()
        {
            var catalog =
                CreateDefaultConventionCatalog();

            var importConvention =
                catalog.Conventions.First().Imports.First();

            importConvention.ContractType = x => typeof(object);

            var part =
                catalog.Parts.First();

            var import =
                part.ImportDefinitions.Cast<ContractBasedImportDefinition>().First();

            import.RequiredTypeIdentity.ShouldBeNull();
        }

        [Test]
        public void Parts_should_set_type_identity_on_import_definition_when_contract_type_on_import_convention_is_not_null()
        {
            var catalog =
                CreateDefaultConventionCatalog();

            var importConvention =
                catalog.Conventions.First().Imports.First();

            importConvention.Members = x => new[] 
            {
                ReflectionServices.GetProperty<IImportConvention>(z => z.Recomposable)
            };

            var part =
                catalog.Parts.First();

            var import =
                part.ImportDefinitions.Cast<ContractBasedImportDefinition>().First();

            var expectedTypeIdentity =
                AttributedModelServices.GetTypeIdentity(importConvention.ContractType.Invoke(null));

            import.RequiredTypeIdentity.ShouldEqual(expectedTypeIdentity);
        }

        [Test]
        public void Parts_should_set_type_identity_on_import_definition_to_declaring_type_when_contract_type_on_import_convention_is_null_and_member_is_field()
        {
            var catalog =
                CreateDefaultConventionCatalog();

            var importConvention =
                catalog.Conventions.First().Imports.First();

            var member =
                ReflectionServices.GetField<FakePart>(x => x.Count);

            importConvention.ContractType = null;
            importConvention.Members = x => new[] { member };

            var part =
                catalog.Parts.First();

            var import =
                part.ImportDefinitions.Cast<ContractBasedImportDefinition>().First();

            var expectedTypeIdentity =
                AttributedModelServices.GetTypeIdentity(member.GetContractMember());

            import.RequiredTypeIdentity.ShouldEqual(expectedTypeIdentity);
        }

        [Test]
        public void Parts_should_set_type_identity_on_import_definition_to_declaring_type_when_contract_type_on_import_convention_is_null_and_member_is_property()
        {
            var catalog =
                CreateDefaultConventionCatalog();

            var importConvention =
                catalog.Conventions.First().Imports.First();

            var member =
                 ReflectionServices.GetProperty<FakePart>(x => x.Name);

            importConvention.ContractType = null;
            importConvention.Members = x => new[] { member };

            var part =
                catalog.Parts.First();

            var import =
                part.ImportDefinitions.Cast<ContractBasedImportDefinition>().First();

            var expectedTypeIdentity =
                AttributedModelServices.GetTypeIdentity(member.GetContractMember());

            import.RequiredTypeIdentity.ShouldEqual(expectedTypeIdentity);
        }

        [Test]
        public void Parts_should_set_required_metadata_import_definition_from_required_metadata_on_import_convention()
        {
            var catalog =
                CreateDefaultConventionCatalog();

            var importConvention =
                catalog.Conventions.First().Imports.First();

            importConvention.RequiredMetadata =
                new List<RequiredMetadataItem> { new RequiredMetadataItem("Name", typeof(string)) };

            var part =
                catalog.Parts.First();

            var import =
                part.ImportDefinitions.Cast<ContractBasedImportDefinition>().First();

            import.RequiredMetadata.First().Key.ShouldEqual(importConvention.RequiredMetadata.First().Name);
        }

        [Test]
        public void Parts_should_return_same_number_of_import_definitions_as_parameters_when_called_with_constructor_info()
        {
            var catalog =
                CreateDefaultConventionCatalog();

            var importConvention =
                catalog.Conventions.First().Imports.First();

            var member =
                 typeof(FakePart).GetConstructor(new Type[] { typeof(int), typeof(string[]) });

            importConvention.ContractType = null;
            importConvention.Members = x => new[] { member };

            var definitions =
                catalog.Parts.First().ImportDefinitions;

            definitions.Count().ShouldEqual(2);
        }

        [Test]
        public void Parts_should_set_contract_name_on_import_definition_to_contract_name_of_parameter_when_called_with_constructor_info()
        {
            var catalog =
                CreateDefaultConventionCatalog();

            var importConvention =
                catalog.Conventions.First().Imports.First();

            var member =
                 typeof(FakePart).GetConstructor(new Type[] { typeof(int), typeof(string[]) });

            importConvention.Members = x => new[] { member };

            var definitions =
                catalog.Parts.First().ImportDefinitions;

            var expectedContractName =
                AttributedModelServices.GetContractName(member.GetParameters()[0].ParameterType);

            definitions.First().ContractName.ShouldEqual(expectedContractName);
        }

        [Test]
        public void Parts_should_set_contract_type_on_import_definition_to_contract_type_of_parameter_when_called_with_constructor_info()
        {
            var catalog =
                CreateDefaultConventionCatalog();

            var importConvention =
                catalog.Conventions.First().Imports.First();

            var member =
                 typeof(FakePart).GetConstructor(new Type[] { typeof(int), typeof(string[]) });

            importConvention.Members = x => new[] { member };

            var definitions =
                catalog.Parts.First().ImportDefinitions;

            var expectedTypeIdentity =
                AttributedModelServices.GetTypeIdentity(member.GetParameters()[0].ParameterType);

            definitions.Cast<ContractBasedImportDefinition>().First().RequiredTypeIdentity.ShouldEqual(expectedTypeIdentity);
        }

        [Test]
        public void Parts_should_not_set_required_metadata_on_import_convention_when_called_with_constructor_info()
        {
            var catalog =
                CreateDefaultConventionCatalog();

            var importConvention =
                catalog.Conventions.First().Imports.First();

            var member =
                 typeof(FakePart).GetConstructor(new Type[] { typeof(int), typeof(string[]) });

            importConvention.RequiredMetadata = 
                new List<RequiredMetadataItem>
                {
                    new RequiredMetadataItem("foo", typeof(string)),
                    new RequiredMetadataItem("bar", typeof(int))
                };

            importConvention.Members = x => new[] { member };

            var definitions =
                catalog.Parts.First().ImportDefinitions;

            var import =
                definitions.Cast<ContractBasedImportDefinition>().First();

            import.RequiredMetadata.ShouldBeSameAs(Enumerable.Empty<KeyValuePair<string, Type>>());
        }

        [Test]
        public void Parts_should_set_creation_policy_on_import_convention_when_called_with_constructor_info()
        {
            var catalog =
                CreateDefaultConventionCatalog();

            var importConvention =
                catalog.Conventions.First().Imports.First();

            var member =
                 typeof(FakePart).GetConstructor(new Type[] { typeof(int), typeof(string[]) });

            importConvention.Members = x => new[] { member };
            importConvention.CreationPolicy = CreationPolicy.NonShared;

            var definitions =
                catalog.Parts.First().ImportDefinitions;

            var import =
                definitions.Cast<ContractBasedImportDefinition>().First();
            
            import.RequiredCreationPolicy.ShouldEqual(CreationPolicy.NonShared);
        }

        [Test]
        public void Parts_should_set_cardinality_on_import_defintion_to_exaclyone_when_import_convention_not_is_for_collection_and_default_values_are_not_allowed_and_called_with_constructor_info()
        {
            var catalog =
                CreateDefaultConventionCatalog();

            var importConvention =
                catalog.Conventions.First().Imports.First();

            var member =
                 typeof(FakePart).GetConstructor(new Type[] { typeof(int), typeof(string[]) });

            importConvention.Members = x => new[] { member };
            importConvention.AllowDefaultValue = false;

            var definitions =
                catalog.Parts.First().ImportDefinitions;

            var import =
                definitions.Cast<ContractBasedImportDefinition>().First();

            import.Cardinality.ShouldEqual(ImportCardinality.ExactlyOne);
        }

        [Test]
        public void Parts_should_set_cardinality_on_import_defintion_to_zeroorone_when_import_convention_not_is_for_collection_and_default_values_are_allowed_and_called_with_constructor_info()
        {
            var catalog =
                CreateDefaultConventionCatalog();

            var importConvention =
                catalog.Conventions.First().Imports.First();

            var member =
                 typeof(FakePart).GetConstructor(new Type[] { typeof(int), typeof(string[]) });

            importConvention.Members = x => new[] { member };
            importConvention.AllowDefaultValue = true;

            var definitions =
                catalog.Parts.First().ImportDefinitions;

            var import =
                definitions.Cast<ContractBasedImportDefinition>().First();

            import.Cardinality.ShouldEqual(ImportCardinality.ZeroOrOne);
        }

        [Test]
        public void Parts_should_set_cardinality_on_import_defintion_to_zeroormore_when_import_convention_is_for_collection_and_called_with_constructor_info()
        {
            var catalog =
                CreateDefaultConventionCatalog();

            var importConvention =
                catalog.Conventions.First().Imports.First();

            var member =
                 typeof(FakePart).GetConstructor(new Type[] { typeof(int), typeof(string[]) });

            importConvention.Members = x => new[] { member };
            importConvention.AllowDefaultValue = true;

            var definitions =
                catalog.Parts.First().ImportDefinitions;

            var import =
                definitions.Cast<ContractBasedImportDefinition>().Last();

            import.Cardinality.ShouldEqual(ImportCardinality.ZeroOrMore);
        }
    }
}