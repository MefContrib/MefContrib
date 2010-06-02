namespace MefContrib.Hosting.Conventions.Tests
{
    using System;
    using System.ComponentModel.Composition;
    using System.Reflection;
    using NUnit.Framework;

    public class ConventionContractServiceTests
    {
        private readonly IContractService Service;

        public ConventionContractServiceTests()
        {
            this.Service = new ConventionContractService();
        }

        [Test]
        public void GetExportContractName_should_be_virtual()
        {
            var method =
                ReflectionServices.GetMethod<ConventionContractService>(x => x.GetExportContractName(null, null));

            IsMethodVirtual(method).ShouldBeTrue();
        }

        [Test]
        public void GetExportContractName_should_throw_argumentnullexception_when_called_with_convention_that_is_null()
        {
            var exception =
                Catch.Exception(() => this.Service.GetExportContractName(null, null));

            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Test]
        public void GetExportContractName_should_throw_argumentnullexception_when_called_with_member_that_is_null()
        {
            var exception =
                Catch.Exception(() => this.Service.GetExportContractName(new ExportConvention(), null));

            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Test]
        public void GetExportContractName_should_return_convention_contract_name_when_not_null()
        {
            const string contract = "Contract";

            var convention =
                new ExportConvention
                {
                    ContractName = x => contract,
                    ContractType = x => typeof(string)
                };

            var results = this.Service.GetExportContractName(
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

            var results = this.Service.GetExportContractName(
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

            var results = 
                this.Service.GetExportContractName(convention, member);

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

            var results =
                this.Service.GetExportContractName(convention, member);

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

            var results =
                this.Service.GetExportContractName(convention, member);

            var expectedContractName =
                AttributedModelServices.GetTypeIdentity(member);

            results.ShouldEqual(expectedContractName);
        }

        [Test]
        public void GetExportTypeIdentity_should_be_virtual()
        {
            var method =
                ReflectionServices.GetMethod<ConventionContractService>(x => x.GetExportTypeIdentity(null, null));

            IsMethodVirtual(method).ShouldBeTrue();
        }

        [Test]
        public void GetExportTypeIdentity_should_throw_argumentnullexception_when_called_with_convention_that_is_null()
        {
            var exception =
                Catch.Exception(() => this.Service.GetExportTypeIdentity(null, null));

            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Test]
        public void GetExportTypeIdentity_should_throw_argumentnullexception_when_called_with_member_that_is_null()
        {
            var exception =
                Catch.Exception(() => this.Service.GetExportTypeIdentity(new ExportConvention(), null));

            exception.ShouldBeOfType<ArgumentNullException>();
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

            var result =
                this.Service.GetExportTypeIdentity(convention, ReflectionServices.GetField<FakePart>(x => x.Count));

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

            var result =
                this.Service.GetExportTypeIdentity(convention, member);

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

            var result =
                this.Service.GetExportTypeIdentity(convention, member);

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

            var result =
                this.Service.GetExportTypeIdentity(convention, member);

            var expectedTypeIdentity =
                AttributedModelServices.GetTypeIdentity(member);

            result.ShouldEqual(expectedTypeIdentity);
        }

        [Test]
        public void GetImportContractName_should_be_virtual()
        {
            var method =
                ReflectionServices.GetMethod<ConventionContractService>(x => x.GetImportContractName(null, null));

            IsMethodVirtual(method).ShouldBeTrue();
        }

        [Test]
        public void GetImportContractName_should_throw_argumentnullexception_when_called_with_convention_that_is_null()
        {
            var exception =
                Catch.Exception(() => this.Service.GetImportContractName(null, null));

            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Test]
        public void GetImportContractName_should_throw_argumentnullexception_when_called_with_member_that_is_null()
        {
            var exception =
                Catch.Exception(() => this.Service.GetImportContractName(new ImportConvention(), null));

            exception.ShouldBeOfType<ArgumentNullException>();
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

            var result = this.Service.GetImportContractName(
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

            var result = this.Service.GetImportContractName(
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

            var results =
                this.Service.GetImportContractName(convention, member);

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

            var results =
                this.Service.GetImportContractName(convention, member);

            var expectedContractName =
                AttributedModelServices.GetContractName(member.PropertyType);

            results.ShouldEqual(expectedContractName);
        }

        [Test]
        public void GetImportTypeIdentity_should_be_virtual()
        {
            var method =
                ReflectionServices.GetMethod<ConventionContractService>(x => x.GetImportTypeIdentity(null, null));

            IsMethodVirtual(method).ShouldBeTrue();
        }

        [Test]
        public void GetImportTypeIdentity_should_throw_argumentnullexception_when_called_with_convention_that_is_null()
        {
            var exception =
                Catch.Exception(() => this.Service.GetImportTypeIdentity(null, null));

            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Test]
        public void GetImportTypeIdentity_should_throw_argumentnullexception_when_called_with_member_that_is_null()
        {
            var exception =
                Catch.Exception(() => this.Service.GetImportTypeIdentity(new ImportConvention(), null));

            exception.ShouldBeOfType<ArgumentNullException>();
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

            var results = 
                this.Service.GetImportTypeIdentity(convention, member);

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

            var results =
                this.Service.GetImportTypeIdentity(convention, member);

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

            var results = this.Service.GetImportTypeIdentity(
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

            var results = this.Service.GetImportTypeIdentity(
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

            var results = 
                this.Service.GetImportTypeIdentity(convention, member);

            var expectedTypeIdentity =
                AttributedModelServices.GetTypeIdentity(member.PropertyType.GetMethod("Invoke"));

            results.ShouldEqual(expectedTypeIdentity);
        }

        private static bool IsMethodVirtual(MethodBase method)
        {
            return (method.IsVirtual && !method.IsFinal);
        }
    }
}