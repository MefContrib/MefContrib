using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using NUnit.Framework;

namespace MefContrib.Hosting.Generics.Tests
{
    [TestFixture]
    public class GenericTypeCatalogTests
    {
        [Test]
        public void Passing_open_generic_type_as_the_exporting_member_causes_an_exception()
        {
            Assert.That(() =>
            {
                new GenericTypeCatalog(typeof(Service1<>));
            }, Throws.InstanceOf<ArgumentException>());
        }

        [Test]
        public void Passing_closed_generic_type_as_the_contract_type_causes_an_exception()
        {
            Assert.That(() =>
            {
                new GenericTypeCatalog(typeof(Service1<Customer>), typeof(IService<Customer>));
            }, Throws.InstanceOf<ArgumentException>());
        }

        [Test]
        public void Passing_null_to_the_ctor_causes_an_exception()
        {
            Assert.That(() =>
            {
                new GenericTypeCatalog(null);
            }, Throws.InstanceOf<ArgumentException>());

            Assert.That(() =>
            {
                new GenericTypeCatalog(typeof(Service1<Customer>), null);
            }, Throws.InstanceOf<ArgumentException>());
        }

        [Test]
        public void When_querying_for_a_generic_part_exported_by_the_export_attribute_with_default_contract_name_and_type_the_closed_generic_part_is_returned()
        {
            var typeCatalog = new GenericTypeCatalog(typeof(Service1<Customer>));
            var parts = typeCatalog.Parts.ToList();
            
            Assert.That(parts.Count, Is.EqualTo(1));
            Assert.That(parts[0].ExportDefinitions.First().ContractName,
                Is.EqualTo(AttributedModelServices.GetContractName(typeof(Service1<Customer>))));
            Assert.That(parts[0].ExportDefinitions.First().Metadata[CompositionConstants.ExportTypeIdentityMetadataName],
                Is.EqualTo(AttributedModelServices.GetTypeIdentity(typeof(Service1<Customer>))));

            // Assert other export definitions
            Assert.That(parts[0].ExportDefinitions.Skip(1).First().ContractName,
                Is.EqualTo("Foo-Bool"));
            Assert.That(parts[0].ExportDefinitions.Skip(1).First().Metadata[CompositionConstants.ExportTypeIdentityMetadataName],
                Is.EqualTo(AttributedModelServices.GetTypeIdentity(typeof(bool))));

            Assert.That(parts[0].ExportDefinitions.Skip(2).First().ContractName,
                Is.EqualTo("Foo-String"));
            Assert.That(parts[0].ExportDefinitions.Skip(2).First().Metadata[CompositionConstants.ExportTypeIdentityMetadataName],
                Is.EqualTo(AttributedModelServices.GetTypeIdentity(typeof(string))));

            Assert.That(parts[0].ExportDefinitions.Skip(3).First().ContractName,
                Is.EqualTo("Foo-Method"));
            Assert.That(parts[0].ExportDefinitions.Skip(3).First().Metadata[CompositionConstants.ExportTypeIdentityMetadataName],
                Is.EqualTo(AttributedModelServices.GetTypeIdentity(typeof(Action))));
        }

        [Test]
        public void When_querying_for_a_generic_part_exported_by_the_export_attribute_with_a_given_contract_name_and_default_contract_type_the_closed_generic_part_is_returned()
        {
            var typeCatalog = new GenericTypeCatalog(typeof(Service2<Customer>));
            var parts = typeCatalog.Parts.ToList();

            Assert.That(parts.Count, Is.EqualTo(1));
            Assert.That(parts[0].ExportDefinitions.First().ContractName,
                Is.EqualTo("contract-name"));
            Assert.That(parts[0].ExportDefinitions.First().Metadata[CompositionConstants.ExportTypeIdentityMetadataName],
                Is.EqualTo(AttributedModelServices.GetTypeIdentity(typeof(Service2<Customer>))));
        }

        [Test]
        public void When_querying_for_a_generic_part_exported_by_the_export_attribute_with_default_contract_name_and_given_contract_type_the_closed_generic_part_is_returned()
        {
            var typeCatalog = new GenericTypeCatalog(typeof(ServiceImpl1<Customer>), typeof(IService<>));
            var parts = typeCatalog.Parts.ToList();

            Assert.That(parts.Count, Is.EqualTo(1));
            Assert.That(parts[0].ExportDefinitions.First().ContractName,
                Is.EqualTo(AttributedModelServices.GetContractName(typeof(IService<Customer>))));
            Assert.That(parts[0].ExportDefinitions.First().Metadata[CompositionConstants.ExportTypeIdentityMetadataName],
                Is.EqualTo(AttributedModelServices.GetTypeIdentity(typeof(IService<Customer>))));

            // Assert other export definitions
            Assert.That(parts[0].ExportDefinitions.Skip(1).First().ContractName,
                Is.EqualTo("Foo-Bool"));
            Assert.That(parts[0].ExportDefinitions.Skip(1).First().Metadata[CompositionConstants.ExportTypeIdentityMetadataName],
                Is.EqualTo(AttributedModelServices.GetTypeIdentity(typeof(bool))));

            Assert.That(parts[0].ExportDefinitions.Skip(2).First().ContractName,
                Is.EqualTo("Foo-String"));
            Assert.That(parts[0].ExportDefinitions.Skip(2).First().Metadata[CompositionConstants.ExportTypeIdentityMetadataName],
                Is.EqualTo(AttributedModelServices.GetTypeIdentity(typeof(string))));

            Assert.That(parts[0].ExportDefinitions.Skip(3).First().ContractName,
                Is.EqualTo("Foo-Method"));
            Assert.That(parts[0].ExportDefinitions.Skip(3).First().Metadata[CompositionConstants.ExportTypeIdentityMetadataName],
                Is.EqualTo(AttributedModelServices.GetTypeIdentity(typeof(Action))));
        }

        [Test]
        public void When_querying_for_a_generic_part_exported_by_the_export_attribute_with_a_given_contract_name_and_type_the_closed_generic_part_is_returned()
        {
            var typeCatalog = new GenericTypeCatalog(typeof(ServiceImpl2<Customer>), typeof(IService<>));
            var parts = typeCatalog.Parts.ToList();

            Assert.That(parts.Count, Is.EqualTo(1));
            Assert.That(parts[0].ExportDefinitions.First().ContractName,
                Is.EqualTo("contract-name-2"));
            Assert.That(parts[0].ExportDefinitions.First().Metadata[CompositionConstants.ExportTypeIdentityMetadataName],
                Is.EqualTo(AttributedModelServices.GetTypeIdentity(typeof(IService<Customer>))));
        }

        [Test]
        public void When_querying_for_a_generic_part_exported_by_the_inherited_export_attribute_with_default_contract_name_and_type_the_closed_generic_part_is_returned()
        {
            var typeCatalog = new GenericTypeCatalog(typeof(InheritedService1Impl<Customer>), typeof(IInheritedService1<>));
            var parts = typeCatalog.Parts.ToList();

            Assert.That(parts.Count, Is.EqualTo(1));
            Assert.That(parts[0].ExportDefinitions.Skip(3).First().ContractName,
                Is.EqualTo(AttributedModelServices.GetContractName(typeof(IInheritedService1<Customer>))));
            Assert.That(parts[0].ExportDefinitions.Skip(3).First().Metadata[CompositionConstants.ExportTypeIdentityMetadataName],
                Is.EqualTo(AttributedModelServices.GetTypeIdentity(typeof(IInheritedService1<Customer>))));
        }

        [Test]
        public void When_querying_for_a_generic_part_exported_by_the_inherited_export_attribute_with_a_given_contract_name_and_default_contract_type_the_closed_generic_part_is_returned()
        {
            var typeCatalog = new GenericTypeCatalog(typeof(InheritedService2Impl<Customer>), typeof(IInheritedService2<>));
            var parts = typeCatalog.Parts.ToList();

            Assert.That(parts.Count, Is.EqualTo(1));
            Assert.That(parts[0].ExportDefinitions.Skip(3).First().ContractName,
                Is.EqualTo("contract-name"));
            Assert.That(parts[0].ExportDefinitions.Skip(3).First().Metadata[CompositionConstants.ExportTypeIdentityMetadataName],
                Is.EqualTo(AttributedModelServices.GetTypeIdentity(typeof(IInheritedService2<Customer>))));
        }

        [Test]
        public void When_querying_for_a_generic_part_exported_by_the_inherited_export_attribute_with_a_given_contract_name_and_type_the_closed_generic_part_is_returned()
        {
            var typeCatalog = new GenericTypeCatalog(typeof(InheritedService3Impl<Customer>), typeof(IService<>));
            var parts = typeCatalog.Parts.ToList();

            Assert.That(parts.Count, Is.EqualTo(1));
            Assert.That(parts[0].ExportDefinitions.Skip(3).First().ContractName,
                Is.EqualTo("contract-name-2"));
            Assert.That(parts[0].ExportDefinitions.Skip(3).First().Metadata[CompositionConstants.ExportTypeIdentityMetadataName],
                Is.EqualTo(AttributedModelServices.GetTypeIdentity(typeof(IService<Customer>))));
        }
    }
    
    [Export]
    public class Service1<T>
    {
        [Export("Foo-Method")]
        public void FooMethod() {}

        [Export("Foo-String")]
        public string FooProperty
        {
            get { return "foo"; }
        }

        [Export("Foo-Bool")]
        private bool fooValue = true;
    }

    [Export("contract-name")]
    public class Service2<T>
    {
        [Export("Foo-Method")]
        public void FooMethod() { }

        [Export("Foo-String")]
        public string FooProperty
        {
            get { return "foo"; }
        }

        [Export("Foo-Bool")]
        private bool fooValue = true;
    }

    [Export(typeof(IService<>))]
    public class ServiceImpl1<T> : IService<T>
    {
        [Export("Foo-Method")]
        public void FooMethod() { }

        [Export("Foo-String")]
        public string FooProperty
        {
            get { return "foo"; }
        }

        [Export("Foo-Bool")]
        private bool fooValue = true;
    }

    [Export("contract-name-2", typeof(IService<>))]
    public class ServiceImpl2<T> : IService<T>
    {
        [Export("Foo-Method")]
        public void FooMethod() { }

        [Export("Foo-String")]
        public string FooProperty
        {
            get { return "foo"; }
        }

        [Export("Foo-Bool")]
        private bool fooValue = true;
    }

    public interface IService<T> { }

    [InheritedExport]
    public interface IInheritedService1<T> { }

    public class InheritedService1Impl<T> : IInheritedService1<T>
    {
        [Export("Foo-Method")]
        public void FooMethod() { }

        [Export("Foo-String")]
        public string FooProperty
        {
            get { return "foo"; }
        }

        [Export("Foo-Bool")]
        private bool fooValue = true;
    }

    [InheritedExport("contract-name")]
    public interface IInheritedService2<T> { }

    public class InheritedService2Impl<T> : IInheritedService2<T>
    {
        [Export("Foo-Method")]
        public void FooMethod() { }

        [Export("Foo-String")]
        public string FooProperty
        {
            get { return "foo"; }
        }

        [Export("Foo-Bool")]
        private bool fooValue = true;
    }

    [InheritedExport("contract-name-2", typeof(IService<>))]
    public interface IInheritedService3<T> { }

    public class InheritedService3Impl<T> : IInheritedService3<T>, IService<T>
    {
        [Export("Foo-Method")]
        public void FooMethod() { }

        [Export("Foo-String")]
        public string FooProperty
        {
            get { return "foo"; }
        }

        [Export("Foo-Bool")]
        private bool fooValue = true;
    }

    public class Customer { }
}