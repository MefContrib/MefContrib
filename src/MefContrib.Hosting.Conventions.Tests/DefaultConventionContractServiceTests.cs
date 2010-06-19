namespace MefContrib.Hosting.Conventions.Tests
{
    using System;
    using System.ComponentModel.Composition;
    using MefContrib.Hosting.Conventions.Configuration;
    using NUnit.Framework;

    public class DefaultConventionContractServiceTests
    {
        [Test]
        public void DefaultConventions_property_should_not_be_null_on_new_instance()
        {
            GetServiceWithDefaultConventions().DefaultConventions.ShouldNotBeNull();
        }

        [Test]
        public void GetExportContractName_should_throw_argumentnullexception_when_called_with_convention_that_is_null()
        {
            var exception =
                Catch.Exception(() => GetServiceWithDefaultConventions().GetExportContractName(null, null));

            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Test]
        public void GetExportContractName_should_throw_argumentnullexception_when_called_with_member_that_is_null()
        {
            var exception =
                Catch.Exception(() => GetServiceWithDefaultConventions().GetExportContractName(new ExportConvention(), null));

            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Test]
        public void GetExportContractName_should_return_default_contract_name_when_match_is_available()
        {
            var member = 
                ReflectionServices.GetProperty<FakePart>(x => x.Name);

            var convention =
                new ExportConvention
                {
                    ContractName = null,
                    ContractType = null,
                    Members = x => new[] { member }
                };

            var result =
                GetServiceWithDefaultConventions().GetExportContractName(convention, member);

            result.ShouldEqual("Foo");
        }

        [Test]
        public void GetExportContractName_should_return_default_contract_name_of_last_default_convention_when_match_is_available()
        {
            const string expectedContractName = "Bar";

            var service =
                GetServiceWithDefaultConventions();

            service.DefaultConventions.Add(
                new TypeDefaultConvention
                {
                    ContractName = expectedContractName,
                    ContractType = typeof(decimal),
                    TargetType = typeof(string)
                });

            var member =
                ReflectionServices.GetProperty<FakePart>(x => x.Name);

            var convention =
                new ExportConvention
                {
                    ContractName = null,
                    ContractType = null,
                    Members = x => new[] { member }
                };

            var result =
                service.GetExportContractName(convention, member);

            result.ShouldEqual(expectedContractName);
        }

        [Test]
        public void GetExportContractName_should_return_convention_contract_name_when_not_null()
        {
            const string contract = "Contract";

            var service =
                GetServiceWithDefaultConventions();

            var convention =
                new ExportConvention
                {
                    ContractName = x => contract,
                    ContractType = x => typeof(string)
                };

            service.DefaultConventions.Clear();

            var results = service.GetExportContractName(
                convention, ReflectionServices.GetField<FakePart>(x => x.Count));

            results.ShouldEqual(contract);
        }

        [Test]
        public void GetExportContractName_should_return_convention_contract_type_when_not_null_and_contract_name_is_null()
        {
            var convention =
                new ExportConvention
                {
                    ContractName = null,
                    ContractType = x => typeof(string)
                };

            var service =
                GetServiceWithoutDefaultConventions();

            var results = service.GetExportContractName(
                convention, ReflectionServices.GetField<FakePart>(x => x.Count));

            var expectedContractName =
                AttributedModelServices.GetContractName(convention.ContractType.Invoke(null));

            results.ShouldEqual(expectedContractName);
        }

        [Test]
        public void GetExportContractName_should_return_contract_name_of_field_type_when_called_with_field_and_contract_name_and_type_are_null()
        {
            var member =
                ReflectionServices.GetField<FakePart>(x => x.Count);

            var convention =
                new ExportConvention
                {
                    ContractName = null,
                    ContractType = null,
                    Members = x => new[] { member }
                };

            var service =
                GetServiceWithoutDefaultConventions();

            var results =
                service.GetExportContractName(convention, member);

            var expectedContractName =
                AttributedModelServices.GetContractName(member.FieldType);

            results.ShouldEqual(expectedContractName);
        }

        [Test]
        public void GetExportContractName_should_return_contract_name_of_property_type_when_called_with_property_and_contract_name_and_type_are_null()
        {
            var member =
                ReflectionServices.GetProperty<FakePart>(x => x.Name);

            var convention =
                new ExportConvention
                {
                    ContractName = null,
                    ContractType = null,
                    Members = x => new[] { member }
                };

            var service =
                GetServiceWithoutDefaultConventions();

            var results =
                service.GetExportContractName(convention, member);

            var expectedContractName =
                AttributedModelServices.GetContractName(member.PropertyType);

            results.ShouldEqual(expectedContractName);
        }

        [Test]
        public void GetExportContractName_should_return_type_identity_of_method_type_when_called_with_method_and_contract_name_and_type_are_null()
        {
            var member =
                ReflectionServices.GetMethod<FakePart>(x => x.DoWork());

            var convention =
                new ExportConvention
                {
                    ContractName = null,
                    ContractType = null,
                    Members = x => new[] { member }
                };

            var service =
                GetServiceWithoutDefaultConventions();

            var results =
                service.GetExportContractName(convention, member);

            var expectedContractName =
                AttributedModelServices.GetTypeIdentity(member);

            results.ShouldEqual(expectedContractName);
        }

        [Test]
        public void GetExportTypeIdentity_should_throw_argumentnullexception_when_called_with_convention_that_is_null()
        {
            var exception =
                Catch.Exception(() => GetServiceWithDefaultConventions().GetExportTypeIdentity(null, null));

            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Test]
        public void GetExportTypeIdentity_should_throw_argumentnullexception_when_called_with_member_that_is_null()
        {
            var exception =
                Catch.Exception(() => GetServiceWithDefaultConventions().GetExportTypeIdentity(new ExportConvention(), null));

            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Test]
        public void GetExportTypeIdentity_should_return_type_identity_of_default_contract_type_when_match_is_available()
        {
            var member =
                ReflectionServices.GetProperty<FakePart>(x => x.Name);

            var convention =
                new ExportConvention
                {
                    ContractName = null,
                    ContractType = null,
                    Members = x => new[] { member }
                };

            var result =
                GetServiceWithDefaultConventions().GetExportTypeIdentity(convention, member);

            var expectedTypeIdentity =
                AttributedModelServices.GetTypeIdentity(typeof(int));

            result.ShouldEqual(expectedTypeIdentity);
        }

        [Test]
        public void GetExportTypeIdentity_should_return_tyoe_identity_of_default_contract_type_of_last_default_convention_when_match_is_available()
        {
            var service =
                GetServiceWithDefaultConventions();

            service.DefaultConventions.Add(
                new TypeDefaultConvention
                {
                    ContractName = string.Empty,
                    ContractType = typeof(decimal),
                    TargetType = typeof(string)
                });

            var member =
                ReflectionServices.GetProperty<FakePart>(x => x.Name);

            var convention =
                new ExportConvention
                {
                    ContractName = null,
                    ContractType = null,
                    Members = x => new[] { member }
                };

            var result =
                service.GetExportTypeIdentity(convention, member);

            var expectedTypeIdentity =
                AttributedModelServices.GetTypeIdentity(typeof(decimal));

            result.ShouldEqual(expectedTypeIdentity);
        }

        [Test]
        public void GetExportTypeIdentity_should_return_type_identity_from_convention_when_not_null()
        {
            var convention =
                new ExportConvention
                {
                    ContractName = x => string.Empty,
                    ContractType = x => typeof(string)
                };

            var service =
                GetServiceWithoutDefaultConventions();

            var result =
                service.GetExportTypeIdentity(convention, ReflectionServices.GetField<FakePart>(x => x.Count));

            var expectedTypeIdentity =
                AttributedModelServices.GetTypeIdentity(typeof(string));

            result.ShouldEqual(expectedTypeIdentity);
        }

        [Test]
        public void GetExportTypeIdentity_should_return_type_identify_of_field_type_when_called_with_field_member_and_convention_contract_type_is_null()
        {
            var convention =
                new ExportConvention
                {
                    ContractName = null,
                    ContractType = null
                };

            var member =
                ReflectionServices.GetField<FakePart>(x => x.Count);

            var service =
                GetServiceWithoutDefaultConventions();

            var result =
                service.GetExportTypeIdentity(convention, member);

            var expectedTypeIdentity =
                AttributedModelServices.GetTypeIdentity(member.FieldType);

            result.ShouldEqual(expectedTypeIdentity);
        }

        [Test]
        public void GetExportTypeIdentity_should_return_type_identify_of_property_type_when_called_with_property_member_and_convention_contract_type_is_null()
        {
            var convention =
                new ExportConvention
                {
                    ContractName = null,
                    ContractType = null
                };

            var member =
                ReflectionServices.GetProperty<FakePart>(x => x.Name);

            var service =
                GetServiceWithoutDefaultConventions();

            var result =
                service.GetExportTypeIdentity(convention, member);

            var expectedTypeIdentity =
                AttributedModelServices.GetTypeIdentity(member.PropertyType);

            result.ShouldEqual(expectedTypeIdentity);
        }

        [Test]
        public void GetExportTypeIdentity_should_return_type_identify_of_method_when_called_with_method_member_and_convention_contract_type_is_null()
        {
            var convention =
                new ExportConvention
                {
                    ContractName = null,
                    ContractType = null
                };

            var member =
                ReflectionServices.GetMethod<FakePart>(x => x.DoWork());

            var service =
                GetServiceWithoutDefaultConventions();

            var result =
                service.GetExportTypeIdentity(convention, member);

            var expectedTypeIdentity =
                AttributedModelServices.GetTypeIdentity(member);

            result.ShouldEqual(expectedTypeIdentity);
        }

        [Test]
        public void GetImportContractName_should_throw_argumentnullexception_when_called_with_convention_that_is_null()
        {
            var exception =
                Catch.Exception(() => GetServiceWithDefaultConventions().GetImportContractName(null, null));

            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Test]
        public void GetImportContractName_should_throw_argumentnullexception_when_called_with_member_that_is_null()
        {
            var exception =
                Catch.Exception(() => GetServiceWithDefaultConventions().GetImportContractName(new ImportConvention(), null));

            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Test]
        public void GetImportContractName_should_return_default_contract_name_when_match_is_available()
        {
            var member =
                ReflectionServices.GetProperty<FakePart>(x => x.Name);

            var convention =
                new ImportConvention()
                {
                    ContractName = null,
                    ContractType = null,
                    Members = x => new[] { member }
                };

            var result =
                GetServiceWithDefaultConventions().GetImportContractName(convention, member);

            result.ShouldEqual("Foo");
        }

        [Test]
        public void GetImportContractName_should_return_default_contract_name_of_last_default_convention_when_match_is_available()
        {
            const string expectedContractName = "Bar";

            var service =
                GetServiceWithDefaultConventions();

            service.DefaultConventions.Add(
                new TypeDefaultConvention
                {
                    ContractName = expectedContractName,
                    ContractType = typeof(decimal),
                    TargetType = typeof(string)
                });

            var member =
                ReflectionServices.GetProperty<FakePart>(x => x.Name);

            var convention =
                new ImportConvention()
                {
                    ContractName = null,
                    ContractType = null,
                    Members = x => new[] { member }
                };

            var result =
                service.GetImportContractName(convention, member);

            result.ShouldEqual(expectedContractName);
        }

        [Test]
        public void GetImportContractName_should_return_contract_name_from_convention_when_not_null()
        {
            const string contract = "Contract";

            var convention =
                new ImportConvention
                {
                    ContractName = x => contract
                };

            var service =
                GetServiceWithoutDefaultConventions();

            var result = service.GetImportContractName(
                convention, ReflectionServices.GetField<FakePart>(x => x.Count));

            result.ShouldEqual(contract);
        }

        [Test]
        public void GetImportContractName_should_return_type_identity_of_convention_contract_type_when_contract_type_is_null()
        {
            var contractType = typeof(string);

            var convention =
                new ImportConvention
                {
                    ContractName = null,
                    ContractType = x => contractType
                };

            var service =
                GetServiceWithoutDefaultConventions();

            var result = service.GetImportContractName(
                convention, ReflectionServices.GetField<FakePart>(x => x.Count));

            var expectedContractName =
                AttributedModelServices.GetTypeIdentity(contractType);

            result.ShouldEqual(expectedContractName);
        }

        [Test]
        public void GetImportContractName_should_return_contract_name_for_field_type_when_called_with_field_and_contract_name_and_type_are_null()
        {
            var member =
                ReflectionServices.GetField<FakePart>(x => x.Count);

            var convention =
                new ImportConvention()
                {
                    ContractName = null,
                    ContractType = null,
                    Members = x => new[] { member }
                };

            var service =
                GetServiceWithoutDefaultConventions();

            var results =
                service.GetImportContractName(convention, member);

            var expectedContractName =
                AttributedModelServices.GetContractName(member.FieldType);

            results.ShouldEqual(expectedContractName);
        }

        [Test]
        public void GetImportContractName_should_return_contract_name_for_property_type_when_called_with_property_and_contract_name_and_type_are_null()
        {
            var member =
                ReflectionServices.GetProperty<FakePart>(x => x.Name);

            var convention =
                new ImportConvention()
                {
                    ContractName = null,
                    ContractType = null,
                    Members = x => new[] { member }
                };

            var service =
                GetServiceWithoutDefaultConventions();

            var results =
                service.GetImportContractName(convention, member);

            var expectedContractName =
                AttributedModelServices.GetContractName(member.PropertyType);

            results.ShouldEqual(expectedContractName);
        }

        [Test]
        public void GetImportTypeIdentity_should_return_type_identity_of_default_contract_type_when_match_is_available()
        {
            var member =
                ReflectionServices.GetProperty<FakePart>(x => x.Name);

            var convention =
                new ImportConvention
                {
                    ContractName = null,
                    ContractType = null,
                    Members = x => new[] { member }
                };

            var result =
                GetServiceWithDefaultConventions().GetImportTypeIdentity(convention, member);

            var expectedTypeIdentity =
                AttributedModelServices.GetTypeIdentity(typeof(int));

            result.ShouldEqual(expectedTypeIdentity);
        }

        [Test]
        public void GetImportTypeIdentity_should_return_tyoe_identity_of_default_contract_type_of_last_default_convention_when_match_is_available()
        {
            var service =
                GetServiceWithDefaultConventions();

            service.DefaultConventions.Add(
                new TypeDefaultConvention
                {
                    ContractName = string.Empty,
                    ContractType = typeof(decimal),
                    TargetType = typeof(string)
                });

            var member =
                ReflectionServices.GetProperty<FakePart>(x => x.Name);

            var convention =
                new ImportConvention()
                {
                    ContractName = null,
                    ContractType = null,
                    Members = x => new[] { member }
                };

            var result =
                service.GetImportTypeIdentity(convention, member);

            var expectedTypeIdentity =
                AttributedModelServices.GetTypeIdentity(typeof(decimal));

            result.ShouldEqual(expectedTypeIdentity);
        }

        [Test]
        public void GetImportTypeIdentity_should_return_type_identity_of_field_type_when_called_with_field_and_contract_type_is_null()
        {
            var member =
                ReflectionServices.GetField<FakePart>(x => x.Count);

            var convention =
                new ImportConvention()
                {
                    ContractName = null,
                    ContractType = null,
                    Members = x => new[] { member }
                };

            var service =
                GetServiceWithoutDefaultConventions();

            var results =
                service.GetImportTypeIdentity(convention, member);

            var expectedTypeIdentity =
                AttributedModelServices.GetTypeIdentity(member.FieldType);

            results.ShouldEqual(expectedTypeIdentity);
        }

        [Test]
        public void GetImportTypeIdentity_should_return_type_identity_of_property_type_when_called_with_property_and_contract_type_is_null()
        {
            var member =
                ReflectionServices.GetProperty<FakePart>(x => x.Name);

            var convention =
                new ImportConvention()
                {
                    ContractName = null,
                    ContractType = null,
                    Members = x => new[] { member }
                };

            var service =
                GetServiceWithoutDefaultConventions();

            var results =
                service.GetImportTypeIdentity(convention, member);

            var expectedTypeIdentity =
                AttributedModelServices.GetTypeIdentity(member.PropertyType);

            results.ShouldEqual(expectedTypeIdentity);
        }

        [Test]
        public void GetImportTypeIdentity_should_return_null_when_convention_contract_type_is_of_type_object()
        {
            var convention =
                new ImportConvention()
                {
                    ContractName = null,
                    ContractType = x => typeof(object)
                };

            var service =
                GetServiceWithoutDefaultConventions();

            var results = service.GetImportTypeIdentity(
                convention, ReflectionServices.GetProperty<FakePart>(x => x.Name));

            results.ShouldBeNull();
        }

        [Test]
        public void GetImportTypeIdentity_should_return_type_identity_of_convention_contract_type_when_contract_type_is_not_object_or_delegate()
        {
            var contractType = typeof(string);

            var convention =
                new ImportConvention()
                {
                    ContractName = null,
                    ContractType = x => contractType
                };

            var service =
                GetServiceWithoutDefaultConventions();

            var results = service.GetImportTypeIdentity(
                convention, ReflectionServices.GetProperty<FakePart>(x => x.Name));

            var expectedTypeIdentity =
                AttributedModelServices.GetTypeIdentity(contractType);

            results.ShouldEqual(expectedTypeIdentity);
        }

        [Test]
        public void GetImportTypeIdentity_should_return_type_identity_of_invoke_method_when_convention_contract_type_is_delegate()
        {
            var member =
                ReflectionServices.GetProperty<FakePart>(x => x.Delegate);

            var convention =
                new ImportConvention()
                {
                    ContractName = null,
                    ContractType = null,
                    Members = x => new[] { member }
                };

            var service =
                GetServiceWithoutDefaultConventions();

            var results =
                service.GetImportTypeIdentity(convention, member);

            var expectedTypeIdentity =
                AttributedModelServices.GetTypeIdentity(member.PropertyType.GetMethod("Invoke"));

            results.ShouldEqual(expectedTypeIdentity);
        }

        [Test]
        public void Configure_should_throw_argumentnullexception_when_called_with_null()
        {
            var service =
                new DefaultConventionContractService();

            var exception =
                Catch.Exception(() => service.Configure(null));

            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Test]
        public void Configure_should_store_default_conventions_to_defaultconventions_property()
        {
            var service =
                new DefaultConventionContractService();

            service.Configure(x => {
                x.ForType<int>().ContractType<int>().ContractName("Foo");
                x.ForType<string>().ContractType<string>().ContractName("Bar");
            });

            service.DefaultConventions.Count.ShouldEqual(2);
        }

        private static DefaultConventionContractService GetServiceWithDefaultConventions()
        {
            var service =
                new DefaultConventionContractService();

            service.DefaultConventions.Add(
                new TypeDefaultConvention
                {
                    ContractName = "Foo",
                    ContractType = typeof(int),
                    TargetType = typeof(string)
                });

            return service;
        }

        private static DefaultConventionContractService GetServiceWithoutDefaultConventions()
        {
            return new DefaultConventionContractService();
        }
    }
}