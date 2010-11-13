using System;
using MefContrib.Hosting.Interception.Handlers;
using NUnit.Framework;

namespace MefContrib.Hosting.Interception.Tests.Handlers
{
    [TestFixture]
    public class GenericContractTypeMappingTests
    {
        [Test]
        public void Generic_contract_type_mapping_is_sealed()
        {
            Assert.That(typeof(GenericContractTypeMapping).IsSealed);
        }

        [Test]
        public void Calling_ctor_with_null_generic_contract_type_definition_causes_argument_null_exception_to_be_thrown()
        {
            Assert.That(delegate
            {
                new GenericContractTypeMapping(null, typeof(IRepository<>));
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Calling_ctor_with_null_generic_implementation_type_definition_causes_argument_null_exception_to_be_thrown()
        {
            Assert.That(delegate
            {
                new GenericContractTypeMapping(typeof(IRepository<>), null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Calling_ctor_with_non_generic_contract_type_definition_causes_argument_exception_to_be_thrown()
        {
            Assert.That(delegate
            {
                new GenericContractTypeMapping(typeof(OrderProcessor), typeof(IRepository<>));
            }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void Calling_ctor_with_non_generic_implementation_type_definition_causes_argument_exception_to_be_thrown()
        {
            Assert.That(delegate
            {
                new GenericContractTypeMapping(typeof(IRepository<>), typeof(OrderProcessor));
            }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void Calling_ctor_with_closed_generic_contract_type_definition_causes_argument_exception_to_be_thrown()
        {
            Assert.That(delegate
            {
                new GenericContractTypeMapping(typeof(IRepository<Order>), typeof(IRepository<>));
            }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void Calling_ctor_with_closed_generic_implementation_type_definition_causes_argument_exception_to_be_thrown()
        {
            Assert.That(delegate
            {
                new GenericContractTypeMapping(typeof(IRepository<>), typeof(IRepository<Order>));
            }, Throws.TypeOf<ArgumentException>());
        }
    }
}