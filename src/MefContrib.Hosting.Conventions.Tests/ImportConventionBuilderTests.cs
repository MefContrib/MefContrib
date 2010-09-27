using MefContrib.Tests;

namespace MefContrib.Hosting.Conventions.Tests
{
    using System;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using MefContrib.Hosting.Conventions.Configuration;
    using NUnit.Framework;

    [TestFixture]
    public class ImportConventionBuilderTests
    {
        private ImportConventionBuilder<ImportConvention> conventionBuilder;

        public ImportConventionBuilderTests()
        {
        }
        
        [SetUp]
        public void Setup()
        {
            this.conventionBuilder = new ImportConventionBuilder<ImportConvention>(); ;
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
                ReflectionServices.GetProperty<ImportConvention>(x => x.ContractType);

            convention.ContractType.Invoke(property).ShouldBeOfType<ImportConvention>();
        }

        [Test]
        public void ContractType_should_return_reference_to_itself_when_called_with_type()
        {
            var reference =
                this.conventionBuilder
                    .ContractType<IImportConvention>();

            reference.ShouldBeSameAs(this.conventionBuilder);
        }

        [Test]
        public void ContractType_should_set_contract_type_on_convention_when_called_with_type()
        {
            this.conventionBuilder
                .ContractType<IImportConvention>();

            var convention =
                this.conventionBuilder
                    .GetBuiltInstance();

            convention.ContractType.Invoke(null).ShouldBeOfType<IImportConvention>();
        }

        [Test]
        public void ContractName_should_return_reference_to_itself_when_called_with_type()
        {
            var reference =
                this.conventionBuilder
                    .ContractName<IImportConvention>();

            reference.ShouldBeSameAs(this.conventionBuilder);
        }

        [Test]
        public void ContractName_should_set_contract_name_on_convention_when_called_with_type()
        {
            this.conventionBuilder
                .ContractName<IImportConvention>();

            var convention =
                this.conventionBuilder
                    .GetBuiltInstance();

            var expectedContractName =
                typeof(IImportConvention).FullName;

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
                ReflectionServices.GetProperty<ImportConvention>(x => x.ContractName);

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
        public void AllowDefaultValue_should_return_reference_to_itself()
        {
            var reference =
                this.conventionBuilder.AllowDefaultValue();

            reference.ShouldBeSameAs(this.conventionBuilder);
        }

        [Test]
        public void AllowDefaultValue_should_be_false_on_new_convention()
        {
            var convention = 
                this.conventionBuilder.GetBuiltInstance();

            convention.AllowDefaultValue.ShouldBeFalse();
        }

        [Test]
        public void AllowDefaultValue_should_set_allow_default_value_on_convention_to_true()
        {
            this.conventionBuilder
                .AllowDefaultValue();

            var convention = 
                this.conventionBuilder.GetBuiltInstance();

            convention.AllowDefaultValue.ShouldBeTrue();
        }

        [Test]
        public void ContractName_should_be_null_on_new_convention()
        {
            var convention =
                this.conventionBuilder.GetBuiltInstance();

            convention.ContractName.ShouldBeNull();
        }

        [Test]
        public void ContractType_should_be_null_on_new_convention()
        {
            var convention =
                this.conventionBuilder.GetBuiltInstance();

            convention.ContractType.ShouldBeNull();
        }

        [Test]
        public void CreationPolicy_should_be_any_on_new_convention()
        {
            var convention =
                this.conventionBuilder.GetBuiltInstance();

            convention.CreationPolicy.ShouldEqual(CreationPolicy.Any);
        }

        [Test]
        public void MakeNonShared_should_return_reference_to_itself()
        {
            var reference =
                this.conventionBuilder.MakeNonShared();

            reference.ShouldBeSameAs(this.conventionBuilder);
        }

        [Test]
        public void MakeNonShared_should_set_creation_policy_on_convention_to_non_shared()
        {
            this.conventionBuilder
                .MakeNonShared();
            
            var convention = 
                this.conventionBuilder.GetBuiltInstance();

            convention.CreationPolicy.ShouldEqual(CreationPolicy.NonShared);
        }

        [Test]
        public void MakeShared_should_return_reference_to_itself()
        {
            var reference =
                this.conventionBuilder.MakeShared();

            reference.ShouldBeSameAs(this.conventionBuilder);
        }

        [Test]
        public void MakeShared_should_set_creation_policy_on_convention_to_non_shared()
        {
            this.conventionBuilder
                .MakeShared();

            var convention = 
                this.conventionBuilder.GetBuiltInstance();

            convention.CreationPolicy.ShouldEqual(CreationPolicy.Shared);
        }

        [Test]
        public void Recomposable_should_return_reference_to_itself()
        {
            var reference =
                this.conventionBuilder.Recomposable();

            reference.ShouldBeSameAs(this.conventionBuilder);
        }

        [Test]
        public void Recomposable_should_be_false_on_new_convention()
        {
            var convention =
                this.conventionBuilder.GetBuiltInstance();

            convention.Recomposable.ShouldBeFalse();
        }

        [Test]
        public void Recomposable_should_set_recomposable_on_convention_to_true()
        {
            this.conventionBuilder
                .Recomposable();

            var convention = 
                this.conventionBuilder.GetBuiltInstance();

            convention.Recomposable.ShouldBeTrue();
        }

        [Test]
        public void RequiredMetadata_should_return_reference_to_itself_when_called_with_name_and_type()
        {
            var reference =
                this.conventionBuilder.RequireMetadata<object>("Name");

            reference.ShouldBeSameAs(this.conventionBuilder);
        }

        [Test]
        public void RequireMetadata_should_add_required_metadata_object_to_required_metadata_collection_on_convention_when_called_with_name_and_type()
        {
            this.conventionBuilder
                .RequireMetadata<object>("Name");

            var convention = 
                this.conventionBuilder.GetBuiltInstance();

            var expectedMetadata =
                new RequiredMetadataItem("Name", typeof(object));

            convention.RequiredMetadata.First().ShouldEqual(expectedMetadata);
        }

        [Test]
        public void RequireMetadata_should_throw_argument_null_exception_if_name_is_null()
        {
            var exception =
                Catch.Exception(() => this.conventionBuilder.RequireMetadata<object>(null));

            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Test]
        public void RequireMetadata_should_throw_argument_out_of_range_exception_if_name_is_empty()
        {
            var exception =
                Catch.Exception(() => this.conventionBuilder.RequireMetadata<object>(string.Empty));

            exception.ShouldBeOfType<ArgumentOutOfRangeException>();
        }

        [Test]
        public void RequireMetadata_should_return_reference_to_itself()
        {
            var reference =
                this.conventionBuilder.RequireMetadata<IFakeMetadataView>();

            reference.ShouldBeSameAs(this.conventionBuilder);
        }

        [Test]
        public void RequireMetadata_should_add_public_read_only_properties_from_metadata_view_to_required_metadata_on_convention()
        {
            this.conventionBuilder
                .RequireMetadata<IFakeMetadataView>();

            var convention = 
                this.conventionBuilder.GetBuiltInstance();

            var expectedMetadata =
                typeof(IFakeMetadataView).GetRequiredMetadata();

            var containsAllExpectedMetadata =
                expectedMetadata.All(x => convention.RequiredMetadata.Contains(x));

            containsAllExpectedMetadata.ShouldBeTrue();
        }

        [Test]
        public void Members_should_return_reference_to_itself_when_called_with_function_over_type()
        {
            var reference =
                this.conventionBuilder.Members(x => x.GetProperties());

            reference.ShouldBeSameAs(this.conventionBuilder);
        }

        [Test]
        public void Members_should_throw_argument_null_exception_when_called_with_null()
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
                convention.Members.Invoke(typeof(IImportConvention));

            matchedMembers.Count().ShouldEqual(7);
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
                    .Members<IImportConvention>(x => x.ContractName);

            reference.ShouldBeSameAs(this.conventionBuilder);
        }

        [Test]
        public void Members_should_throw_argument_null_exception_when_called_with_null_value_expression()
        {
            var exception =
                Catch.Exception(() => this.conventionBuilder.Members((Expression<Func<IImportConvention, object>>)null));

            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Test]
        public void Members_should_set_members_on_convention_when_called_with_value_expression()
        {
            this.conventionBuilder
                .Members<IImportConvention>(x => x.ContractName);

            var convention =
                this.conventionBuilder.GetBuiltInstance();

            var matchedMembers =
                convention.Members.Invoke(typeof(IImportConvention));

            matchedMembers.Count().ShouldEqual(1);
        }
    }
}