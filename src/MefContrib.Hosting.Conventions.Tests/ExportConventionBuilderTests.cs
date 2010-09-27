using MefContrib.Tests;

namespace MefContrib.Hosting.Conventions.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using MefContrib.Hosting.Conventions.Configuration;
    using NUnit.Framework;

    [TestFixture]
    public class ExportConventionBuilderTests
    {
        private ExportConventionBuilder<ExportConvention> conventionBuilder;

        public ExportConventionBuilderTests()
        {
        }

        [SetUp]
        public void Setup()
        {
            this.conventionBuilder = new ExportConventionBuilder<ExportConvention>();
        }

        [Test]
        public void ContractType_should_return_reference_to_itself_when_called_with_function()
        {
            var reference =
                this.conventionBuilder
                    .ContractType(x => x.DeclaringType);

            reference.ShouldBeSameAs(this.conventionBuilder);
        }

        [Test]
        public void ContractType_should_throw_argument_null_exception_when_called_with_null_function()
        {
            var exception =
                Catch.Exception(() => this.conventionBuilder.ContractType((Func<MemberInfo, Type>)null));

            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Test]
        public void ContractType_should_set_contract_type_on_convention_when_called_with_function()
        {
            this.conventionBuilder
                .ContractType(x => x.DeclaringType);

            var convention =
                this.conventionBuilder
                    .GetBuiltInstance();

            var property =
                ReflectionServices.GetProperty<ExportConvention>(x => x.ContractType);

            convention.ContractType.Invoke(property).ShouldBeOfType<ExportConvention>();
        }

        [Test]
        public void ContractType_should_return_reference_to_itself_when_called_with_type()
        {
            var reference =
                this.conventionBuilder
                    .ContractType<IExportConvention>();

            reference.ShouldBeSameAs(this.conventionBuilder);
        }

        [Test]
        public void ContractType_should_set_contract_type_on_convention_when_called_with_type()
        {
            this.conventionBuilder
                .ContractType<IExportConvention>();

            var convention =
                this.conventionBuilder
                    .GetBuiltInstance();

            convention.ContractType.Invoke(null).ShouldBeOfType<IExportConvention>();
        }

        [Test]
        public void ContractName_should_return_reference_to_itself_when_called_with_type()
        {
            var reference =
                this.conventionBuilder
                    .ContractName<IExportConvention>();

            reference.ShouldBeSameAs(this.conventionBuilder);
        }

        [Test]
        public void ContractName_should_set_contract_name_on_convention_when_called_with_type()
        {
            this.conventionBuilder
                .ContractName<IExportConvention>();

            var convention =
                this.conventionBuilder
                    .GetBuiltInstance();

            var expectedContractName =
                typeof(IExportConvention).FullName;

            convention.ContractName.Invoke(null).ShouldEqual(expectedContractName);
        }

        [Test]
        public void ContractName_should_return_reference_to_itself_when_called_with_function()
        {
            var reference =
                this.conventionBuilder
                    .ContractName(x => x.Name);

            reference.ShouldBeSameAs(this.conventionBuilder);
        }

        [Test]
        public void ContractName_should_throw_argument_null_exception_when_called_with_null_function()
        {
            var exception =
                Catch.Exception(() => this.conventionBuilder.ContractName((Func<MemberInfo, string>)null));

            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Test]
        public void ContractName_should_set_contract_name_on_convention_when_called_with_function()
        {
            this.conventionBuilder
                .ContractName(x => x.Name);

            var convention =
                this.conventionBuilder
                    .GetBuiltInstance();

            var member =
                ReflectionServices.GetProperty<ExportConvention>(x => x.ContractName);

            convention.ContractName.Invoke(member).ShouldEqual(member.Name);
        }

        [Test]
        public void ContractName_should_return_reference_to_itself_when_called_with_string()
        {
            var reference =
                this.conventionBuilder
                    .ContractName("Foo");

            reference.ShouldBeSameAs(this.conventionBuilder);
        }

        [Test]
        public void ContractName_should_throw_argument_out_of_range_exception_when_called_with_empty_string()
        {
            var exception =
                Catch.Exception(() => this.conventionBuilder.ContractName(string.Empty));

            exception.ShouldBeOfType<ArgumentOutOfRangeException>();
        }

        [Test]
        public void ContractName_should_set_contract_name_on_convention_when_called_with_string()
        {
            this.conventionBuilder
                .ContractName("Foo");

            var convention =
                this.conventionBuilder
                    .GetBuiltInstance();

            convention.ContractName.Invoke(null).ShouldEqual("Foo");
        }

        [Test]
        public void GetBuiltInstance_should_not_return_null_on_new_convention()
        {
            var convention =
                this.conventionBuilder.GetBuiltInstance();

            convention.ShouldNotBeNull();
        }

        [Test]
        public void Members_should_return_reference_to_itself_when_called_with_function_over_type()
        {
            var reference =
                this.conventionBuilder
                    .Members(x => x.GetProperties());

            reference.ShouldBeSameAs(this.conventionBuilder);
        }

        [Test]
        public void Members_should_throw_argument_null_exception_when_called_with_null_function_over_type()
        {
            var exception =
                Catch.Exception(() => this.conventionBuilder.Members(null));

            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Test]
        public void Members_should_set_members_on_convention_when_called_with_function_over_type()
        {
            this.conventionBuilder
                .Members(x => x.GetProperties());

            var convention =
                this.conventionBuilder.GetBuiltInstance();

            var matchedMembers =
                convention.Members.Invoke(typeof(IExportConvention));

            matchedMembers.Count().ShouldEqual(4);
        }

        //[Test]
        //public void Members_should_return_reference_to_itself_when_called_with_void_expression()
        //{
        //    var reference =
        //        this.conventionBuilder
        //            .Members<IPartConventionsContainer>(x => x.Configure(null));

        //    reference.ShouldBeSameAs(this.conventionBuilder);
        //}

        //[Test]
        //public void Members_should_throw_argument_null_exception_when_called_with_null_void_expression()
        //{
        //    var exception =
        //        Catch.Exception(() => this.conventionBuilder.Members((Expression<Action<IPartConventionsContainer>>)null));

        //    exception.ShouldBeOfType<ArgumentNullException>();
        //}

        //[Test]
        //public void Members_should_set_members_on_convention_when_called_with_void_expression()
        //{
        //    this.conventionBuilder
        //        .Members<IPartConventionsContainer>(x => x.Configure(null));
                
        //    var convention =
        //        this.conventionBuilder.GetBuiltInstance();

        //    var matchedMembers =
        //        convention.Members.Invoke(typeof(IPartConventionsContainer));

        //    matchedMembers.Count().ShouldEqual(1);
        //}

        [Test]
        public void Members_should_return_reference_to_itself_when_called_with_value_expression()
        {
            var reference =
                this.conventionBuilder
                    .Members<IExportConvention>(x => x.ContractName);

            reference.ShouldBeSameAs(this.conventionBuilder);
        }

        [Test]
        public void Members_should_throw_argument_null_exception_when_called_with_null_value_expression()
        {
            var exception =
                Catch.Exception(() => this.conventionBuilder.Members((Expression<Func<IExportConvention, object>>)null));

            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Test]
        public void Members_should_set_members_on_convention_when_called_with_value_expression()
        {
            this.conventionBuilder
                .Members<IExportConvention>(x => x.ContractName);
            
            var convention =
                this.conventionBuilder.GetBuiltInstance();

            var matchedMembers =
                convention.Members.Invoke(typeof(IExportConvention));

            matchedMembers.Count().ShouldEqual(1);
        }

        [Test]
        public void AddMetadata_should_return_reference_to_itself_when_called_with_name_and_value()
        {
            var reference =
                this.conventionBuilder
                    .AddMetadata("Foo", new object());

            reference.ShouldBeSameAs(this.conventionBuilder);
        }

        [Test]
        public void AddMetadata_should_throw_argument_null_exception_when_called_with_null_name_and_value()
        {
            var exception =
                Catch.Exception(() => this.conventionBuilder.AddMetadata(null, new object()));

            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Test]
        public void AddMetadata_should_throw_argument_out_of_range_exception_when_called_with_empty_name_and_value()
        {
            var exception =
                Catch.Exception(() => this.conventionBuilder.AddMetadata(string.Empty, new object()));

            exception.ShouldBeOfType<ArgumentOutOfRangeException>();
        }

        [Test]
        public void AddMetadata_should_throw_argument_null_exception_when_called_with_name_and_null_value()
        {
            var exception =
                Catch.Exception(() => this.conventionBuilder.AddMetadata("Foo", null));

            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Test]
        public void AddMetadata_should_add_metadata_to_convention()
        {
            this.conventionBuilder
                .AddMetadata("Foo", "Bar");
            
            var convention =
                this.conventionBuilder.GetBuiltInstance();

            var expectedMetadata =
                new MetadataItem("Foo", "Bar");

            convention.Metadata.First().Equals(expectedMetadata).ShouldBeTrue();
        }

        [Test]
        public void AddMetadata_should_return_reference_to_itself_when_called_with_anonymous_type()
        {
            var reference =
                this.conventionBuilder
                    .AddMetadata(new { Foo = "Bar" });

            reference.ShouldBeSameAs(this.conventionBuilder);
        }

        [Test]
        public void AddMetadata_should_throw_argument_null_exception_when_called_with_null_anonymous_type()
        {
            var exception =
                Catch.Exception(() => this.conventionBuilder.AddMetadata((object)null));

            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Test]
        public void AddMetadata_should_set_metadata_on_convention_from_anonymous_type()
        {
            this.conventionBuilder
                .AddMetadata(new { Foo = "Bar" });

            var convention =
                this.conventionBuilder.GetBuiltInstance();

            var expectedMetadata =
                new MetadataItem("Foo", "Bar");

            convention.Metadata.First().Equals(expectedMetadata).ShouldBeTrue();
        }

        [Test]
        public void AddMetadata_should_return_reference_to_itself_when_called_with_a_function()
        {
            var reference =
                this.conventionBuilder
                    .AddMetadata(() => new[] { new KeyValuePair<string, object>("Foo", "Bar") });

            reference.ShouldBeSameAs(this.conventionBuilder);
        }

        [Test]
        public void AddMetadata_should_throw_argument_null_exception_when_called_with_null()
        {
            var exception =
                Catch.Exception(() => this.conventionBuilder.AddMetadata((Func<KeyValuePair<string, object>[]>)null));

            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Test]
        public void AddMetadata_should_throw_argument_null_exception_when_called_with_function_that_returns_null()
        {
            var exception =
                Catch.Exception(() => this.conventionBuilder.AddMetadata(() => null));

            exception.ShouldBeOfType<InvalidOperationException>();
        }

        [Test]
        public void AddMetadata_should_set_metadata_on_convention_when_called_with_a_function()
        {
            this.conventionBuilder
                .AddMetadata(() => new[] {new KeyValuePair<string, object>("Foo", "Bar")});
            
            var convention =
                this.conventionBuilder.GetBuiltInstance();

            var expectedMetadata =
                new MetadataItem("Foo", "Bar");

            convention.Metadata.First().Equals(expectedMetadata).ShouldBeTrue();
        }
    }
}