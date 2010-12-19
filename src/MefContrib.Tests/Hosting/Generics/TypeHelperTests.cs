using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            var implementations = new List<Type>
            {
                typeof (Repository<>)
            };
            var orderRepositoryTypes = TypeHelper.BuildGenericTypes(importDefinitionType, implementations);

            Assert.AreEqual(typeof(Repository<Order>), orderRepositoryTypes.Single());
        }

        [Test]
        public void When_building_a_closed_generic_repository_and_no_implementations_are_present_ArgumentException_is_thrown()
        {
            var importDefinitionType = typeof(IRepository<Order>);
            var implementations = new List<Type>();
            
            Assert.That(() =>
            {
                TypeHelper.BuildGenericTypes(importDefinitionType, implementations);
            }, Throws.InstanceOf<ArgumentException>());
        }

        [Test]
        public void IsCollection_method_test()
        {
            Assert.That(TypeHelper.IsCollection(typeof(int)), Is.False);
            Assert.That(TypeHelper.IsCollection(typeof(string)), Is.False);
            Assert.That(TypeHelper.IsCollection(typeof(IEnumerable)), Is.True);
            Assert.That(TypeHelper.IsCollection(typeof(IEnumerable<string>)), Is.True);
        }

        [Test]
        public void IsGenericCollection_method_test()
        {
            Assert.That(TypeHelper.IsGenericCollection(typeof(int)), Is.False);
            Assert.That(TypeHelper.IsGenericCollection(typeof(string)), Is.False);
            Assert.That(TypeHelper.IsGenericCollection(typeof(IEnumerable)), Is.False);
            Assert.That(TypeHelper.IsGenericCollection(typeof(IEnumerable<string>)), Is.True);
            Assert.That(TypeHelper.IsGenericCollection(typeof(MyClass)), Is.True);
        }

        [Test]
        public void TryGetAncestor_method_test()
        {
            var ancestor = TypeHelper.TryGetAncestor(typeof(IList<string>), typeof(IEnumerable<string>));
            Assert.That(ancestor, Is.Not.Null);
            Assert.That(ancestor.GetGenericArguments()[0], Is.EqualTo(typeof(string)));
        }

        [Test]
        public void TryGetAncestor_method_test_with_open_generics()
        {
            var ancestor = TypeHelper.TryGetAncestor(typeof(IList<string>), typeof(IEnumerable<>));
            Assert.That(ancestor, Is.Not.Null);
            Assert.That(ancestor.GetGenericArguments()[0], Is.EqualTo(typeof(string)));
        }

        [Test]
        public void GetGenericCollectionParameter_method_test()
        {
            var ancestor = TypeHelper.GetGenericCollectionParameter(typeof(MyClass));
            Assert.That(ancestor, Is.Not.Null);
            Assert.That(ancestor, Is.EqualTo(typeof(string)));

            ancestor = TypeHelper.GetGenericCollectionParameter(typeof(MyClass2));
            Assert.That(ancestor, Is.Not.Null);
            Assert.That(ancestor, Is.EqualTo(typeof(string)));
        }

        private class MyClass : List<string> { }

        private class MyClass2 : MyClass { }
    }
}
