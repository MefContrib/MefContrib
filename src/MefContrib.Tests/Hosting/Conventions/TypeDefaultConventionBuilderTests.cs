namespace MefContrib.Hosting.Conventions.Tests
{
    using System;
    using MefContrib.Hosting.Conventions.Configuration;
    using MefContrib.Tests;
    using NUnit.Framework;

    public class TypeDefaultConventionBuilderTests
    {
        [Test]
        public void Ctor_should_throw_argumentnullexception_if_called_with_null()
        {
            var exception =
                Catch.Exception(() => new TypeDefaultConventionBuilder(null));

            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Test]
        public void Ctor_should_persist_target_type_to_target_type_property()
        {
            var builder =
                new TypeDefaultConventionBuilder(typeof(string));

            var convention =
                builder.GetConvention();

            convention.TargetType.ShouldBeOfType<string>();
        }

        [Test]
        public void ContractName_should_return_reference_to_itself()
        {
            const string contractName = "Contract";

            var builder =
                new TypeDefaultConventionBuilder(typeof(string));

            var reference = builder
                .ContractName(contractName);

            reference.ShouldBeSameAs(builder);
        }

        [Test]
        public void ContractName_should_throw_argumentnullexception_if_called_with_null()
        {
            var builder =
                new TypeDefaultConventionBuilder(typeof(string));

            var exception =
                Catch.Exception(() => builder.ContractName(null));

            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Test]
        public void ContractName_should_throw_argumentoutofrangeexception_if_called_wiht_empty_string()
        {
            var builder =
                new TypeDefaultConventionBuilder(typeof(string));

            var exception =
                Catch.Exception(() => builder.ContractName(string.Empty));

            exception.ShouldBeOfType<ArgumentOutOfRangeException>();
        }

        [Test]
        public void ContractName_should_persist_name_in_contract_name_property()
        {
            const string contractName = "Contract";

            var builder =
                new TypeDefaultConventionBuilder(typeof(string));

            builder
                .ContractName(contractName);
            
            var convention =
                builder.GetConvention();

            convention.ContractName.ShouldEqual(contractName);
        }

        [Test]
        public void ContractType_should_return_reference_to_itself()
        {
            var builder =
                new TypeDefaultConventionBuilder(typeof(string));

            var reference = builder
                .ContractType<int>();

            reference.ShouldBeSameAs(builder);
        }

        [Test]
        public void ContractType_should_persist_type_in_contract_type_property()
        {
            var builder =
                new TypeDefaultConventionBuilder(typeof(string));

            builder
                .ContractType<int>();

            var convention =
                builder.GetConvention();

            convention.ContractType.ShouldBeOfType<int>();
        }
    }
}