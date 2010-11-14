using System;
using System.Linq;
using System.ComponentModel.Composition.Hosting;
using MefContrib.Hosting.Interception.Configuration;
using MefContrib.Tests;
using Moq;
using NUnit.Framework;

namespace MefContrib.Hosting.Interception.Tests
{
    [TestFixture]
    public class InterceptingCatalogTests
    {
        private CompositionContainer container;

        [SetUp]
        public void TestSetUp()
        {
            var innerCatalog = new TypeCatalog(typeof(Part1), typeof(Part2), typeof(Part3));
            var cfg = new InterceptionConfiguration()
                .AddInterceptor(new GeneralInterceptor())
                .AddInterceptionCriteria(
                    new PredicateInterceptionCriteria(
                        new PartInterceptor(), d => d.Metadata.ContainsKey("metadata1")));
            var catalog = new InterceptingCatalog(innerCatalog, cfg);
            container = new CompositionContainer(catalog);
        }

        [Test]
        public void Ctor_should_throw_argument_null_exception_if_called_with_null_catalog()
        {
            Assert.That(delegate
            {
                new InterceptingCatalog(null, new InterceptionConfiguration());
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Ctor_should_throw_argument_null_exception_if_called_with_null_configuration()
        {
            Assert.That(delegate
            {
                new InterceptingCatalog(new TypeCatalog(typeof (Part1)), null);
            }, Throws.TypeOf<ArgumentNullException>());
        }
        
        [Test]
        public void When_querying_for_a_part_it_should_return_an_intercepting_part_definition()
        {
            var innerCatalog = new TypeCatalog(typeof(Logger));
            var mockInterceptor = new Mock<IExportedValueInterceptor>();
            var cfg = new InterceptionConfiguration().AddInterceptor(mockInterceptor.Object);
            var catalog = new InterceptingCatalog(innerCatalog, cfg);

            var partDefinition = catalog.Parts.First();
            partDefinition.ShouldBeOfType<InterceptingComposablePartDefinition>();
        }

        [Test]
        public void When_querying_for_a_part_not_being_intercepted_it_should_return_original_part_definition()
        {
            var innerCatalog = new TypeCatalog(typeof(Logger));
            var cfg = new InterceptionConfiguration();
            var catalog = new InterceptingCatalog(innerCatalog, cfg);

            var partDefinition = catalog.Parts.First();
            partDefinition.ShouldNotBeOfType<InterceptingComposablePartDefinition>();
        }

        [Test]
        public void Catalog_should_return_non_intercepted_value_for_part1()
        {
            var part = container.GetExportedValue<IPart>();
            Assert.That(part.GetType(), Is.EqualTo(typeof(Part1)));
        }

        [Test]
        public void Catalog_should_return_an_intercepted_value_for_part2()
        {
            var part = container.GetExportedValue<IPart>("part2");
            Assert.That(part.GetType(), Is.EqualTo(typeof(PartWrapper)));
            Assert.That(((PartWrapper)part).Inner.GetType(), Is.EqualTo(typeof(Part2)));
        }

        [Test]
        public void Catalog_should_return_a_part_with_properly_set_name()
        {
            var part1 = container.GetExportedValue<IPart>();
            var part2 = container.GetExportedValue<IPart>("part2");

            Assert.That(part1.Name, Is.EqualTo("Name property is set be the interceptor."));
            Assert.That(part2.Name, Is.EqualTo("Name property is set be the interceptor."));
        }

        [Test]
        public void Catalog_should_return_a_part_with_respect_to_its_creation_policy()
        {
            Part1.InstanceCount = 0;
            Part3.InstanceCount = 0;

            var part11 = container.GetExportedValue<IPart>();
            var part12 = container.GetExportedValue<IPart>();
            var part31 = container.GetExportedValue<IPart>("part3");
            var part32 = container.GetExportedValue<IPart>("part3");

            Assert.That(part11, Is.Not.Null);
            Assert.That(part12, Is.Not.Null);
            Assert.That(part31, Is.Not.Null);
            Assert.That(part32, Is.Not.Null);

            Assert.That(part11, Is.SameAs(part12));
            Assert.That(part31, Is.Not.SameAs(part32));

            Assert.That(Part1.InstanceCount, Is.EqualTo(1));
            Assert.That(Part3.InstanceCount, Is.EqualTo(2));
        }

        [Test]
        public void Catalog_should_filter_out_parts()
        {
            var innerCatalog = new TypeCatalog(typeof(Part0), typeof(Part1), typeof(Part2), typeof(Part3));
            var cfg = new InterceptionConfiguration()
                .AddHandler(new PartFilter());
            var catalog = new InterceptingCatalog(innerCatalog, cfg);
            container = new CompositionContainer(catalog);

            var parts = container.GetExportedValues<IPart>();
            Assert.That(parts.Count(), Is.EqualTo(1));
            Assert.That(parts.First().GetType(), Is.EqualTo(typeof(Part0)));
        }

        [Test]
        public void Disposing_catalog_should_dispose_parts_implementing_dispose_pattern()
        {
            var innerCatalog = new TypeCatalog(typeof(DisposablePart));
            var cfg = new InterceptionConfiguration()
                .AddInterceptor(new FakeInterceptor());
            var catalog = new InterceptingCatalog(innerCatalog, cfg);
            var partDefinition = catalog.Parts.First();
            container = new CompositionContainer(catalog);

            partDefinition.ShouldBeOfType<InterceptingComposablePartDefinition>();

            var part = container.GetExportedValueOrDefault<DisposablePart>();
            Assert.That(part.IsDisposed, Is.False);
            container.Dispose();
            Assert.That(part.IsDisposed, Is.True);
        }
    }
}