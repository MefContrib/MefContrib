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
        public void Passing_open_generic_type_causes_an_exception()
        {
            Assert.That(() =>
            {
                new GenericTypeCatalog(typeof(Service1<>));
            }, Throws.InstanceOf<ArgumentException>());
        }

        [Test]
        public void When_querying_for_a_generic_part_exported_by_the_export_attribute_with_default_contract_name_and_type_the_closed_generic_part_is_returned()
        {
            var typeCatalog = new GenericTypeCatalog(typeof(Service1<Customer>));
            var parts = typeCatalog.Parts.ToList();
            
            Assert.That(parts.Count, Is.EqualTo(1));
            Assert.That(parts[0].ExportDefinitions.Single().ContractName,
                Is.EqualTo(AttributedModelServices.GetContractName(typeof(Service1<Customer>))));
            Assert.That(parts[0].ExportDefinitions.Single().Metadata[CompositionConstants.ExportTypeIdentityMetadataName],
                Is.EqualTo(AttributedModelServices.GetTypeIdentity(typeof(Service1<Customer>))));
        }

        [Test]
        public void When_querying_for_a_generic_part_exported_by_the_export_attribute_with_a_given_contract_name_and_default_contract_type_the_closed_generic_part_is_returned()
        {
            var typeCatalog = new GenericTypeCatalog(typeof(Service2<Customer>));
            var parts = typeCatalog.Parts.ToList();

            Assert.That(parts.Count, Is.EqualTo(1));
            Assert.That(parts[0].ExportDefinitions.Single().ContractName,
                Is.EqualTo("contract-name"));
            Assert.That(parts[0].ExportDefinitions.Single().Metadata[CompositionConstants.ExportTypeIdentityMetadataName],
                Is.EqualTo(AttributedModelServices.GetTypeIdentity(typeof(Service2<Customer>))));
        }

        [Test]
        public void When_querying_for_a_generic_part_exported_by_the_export_attribute_with_default_contract_name_and_given_contract_type_the_closed_generic_part_is_returned()
        {
            var typeCatalog = new GenericTypeCatalog(typeof(ServiceImpl1<Customer>), typeof(IService<>));
            var parts = typeCatalog.Parts.ToList();

            Assert.That(parts.Count, Is.EqualTo(1));
            Assert.That(parts[0].ExportDefinitions.Single().ContractName,
                Is.EqualTo(AttributedModelServices.GetContractName(typeof(IService<Customer>))));
            Assert.That(parts[0].ExportDefinitions.Single().Metadata[CompositionConstants.ExportTypeIdentityMetadataName],
                Is.EqualTo(AttributedModelServices.GetTypeIdentity(typeof(IService<Customer>))));
        }

        [Test]
        public void When_querying_for_a_generic_part_exported_by_the_export_attribute_with_a_given_contract_name_and_type_the_closed_generic_part_is_returned()
        {
            var typeCatalog = new GenericTypeCatalog(typeof(ServiceImpl2<Customer>), typeof(IService<>));
            var parts = typeCatalog.Parts.ToList();

            Assert.That(parts.Count, Is.EqualTo(1));
            Assert.That(parts[0].ExportDefinitions.Single().ContractName,
                Is.EqualTo("contract-name-2"));
            Assert.That(parts[0].ExportDefinitions.Single().Metadata[CompositionConstants.ExportTypeIdentityMetadataName],
                Is.EqualTo(AttributedModelServices.GetTypeIdentity(typeof(IService<Customer>))));
        }
    }

    [Export(typeof(IS))]
    public class S : IS {}
    
    public interface IS {}

    [Export]
    public class Service1<T>
    {
    }

    [Export("contract-name")]
    public class Service2<T>
    {
    }

    [Export(typeof(IService<>))]
    public class ServiceImpl1<T> : IService<T>
    {
    }

    [Export("contract-name-2", typeof(IService<>))]
    public class ServiceImpl2<T> : IService<T>
    {
    }

    public interface IService<T> { }

    public class Customer { }
}