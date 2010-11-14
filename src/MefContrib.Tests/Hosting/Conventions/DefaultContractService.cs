namespace MefContrib.Hosting.Conventions.Tests
{
    using System;
    using System.ComponentModel.Composition;
    using System.Reflection;
    using MefContrib.Tests;
    using NUnit.Framework;

    public class DefaultContractServiceTests
    {
        private readonly IContractService Service;

        public DefaultContractServiceTests()
        {
            this.Service = new DefaultConventionContractService();
        }

        [Test]
        public void GetExportContractName_should_be_virtual()
        {
            var method =
                ReflectionServices.GetMethod<DefaultConventionContractService>(x => x.GetExportContractName(null, null));

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
        public void GetExportContractName_should_throw_argumentnullexception_when_called_with_null_member()
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
                ReflectionServices.GetMethod<DefaultConventionContractService>(x => x.GetExportTypeIdentity(null, null));

            IsMethodVirtual(method).ShouldBeTrue();
        }

        [Test]
        public void GetImportContractName_should_be_virtual()
        {
            var method =
                ReflectionServices.GetMethod<DefaultConventionContractService>(x => x.GetImportContractName(null, null));

            IsMethodVirtual(method).ShouldBeTrue();
        }

        [Test]
        public void GetImportTypeIdentity_should_be_virtual()
        {
            var method =
                ReflectionServices.GetMethod<DefaultConventionContractService>(x => x.GetImportTypeIdentity(null, null));

            IsMethodVirtual(method).ShouldBeTrue();
        }

        private static bool IsMethodVirtual(MethodBase method)
        {
            return (method.IsVirtual && !method.IsFinal);
        }
    }
}