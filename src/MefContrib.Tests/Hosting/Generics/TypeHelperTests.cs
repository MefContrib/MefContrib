using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace MefContrib.Hosting.Generics.Tests
{
    [TestFixture]
    public class TypeHelperTests
    {
        [Test]
        public void When_building_a_closed_generic_repository_Order_repository_is_returned()
        {
            var importDefinitionType = typeof(IRepository<Order>);
            var typeMapping = new Dictionary<Type, Type>
            {
                {typeof (IRepository<>), typeof (Repository<>)}
            };
            var orderRepositoryType = TypeHelper.BuildGenericType(importDefinitionType, typeMapping);

            Assert.AreEqual(typeof(Repository<Order>), orderRepositoryType);
        }

        [Test]
        public void When_building_a_closed_generic_repository_and_no_mapping_is_present_MappingNotFoundException_is_thrown()
        {
            var importDefinitionType = typeof(IRepository<Order>);
            var typeMapping = new Dictionary<Type, Type>();

            Type mappingTypeFromException = null;

            try
            {
                TypeHelper.BuildGenericType(importDefinitionType, typeMapping);
            }
            catch (MappingNotFoundException ex)
            {
                mappingTypeFromException = ex.Type;
            }

            Assert.That(mappingTypeFromException, Is.SameAs(typeof(IRepository<>)));
        }
    }
}
