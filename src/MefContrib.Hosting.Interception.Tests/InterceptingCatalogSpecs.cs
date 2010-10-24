using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.ComponentModel.Composition.Hosting;
using MefContrib.Hosting.Interception.Configuration;
using MefContrib.Tests;
using Moq;
using NUnit.Framework;

namespace MefContrib.Hosting.Interception.Tests
{
    namespace Given_an_InterceptingCatalog
    {
        [TestFixture]
        public class when_querying_for_a_part : InterceptingCatalogContext
        {
            [Test]
            public void it_should_return_an_intercepting_part_definition()
            {
                PartDefinition.ShouldBeOfType<InterceptingComposablePartDefinition>();
            }

            public override void Context()
            {
                PartDefinition = Catalog.Parts.First();
            }
        }

        public class InterceptingCatalogContext
        {
            public InterceptingCatalog Catalog;
            public Mock<IExportedValueInterceptor> MockInterceptor;
            public ComposablePartDefinition PartDefinition;

            public InterceptingCatalogContext()
            {
                var innerCatalog = new TypeCatalog(typeof(Logger));
                MockInterceptor = new Mock<IExportedValueInterceptor>();
                var cfg = new InterceptionConfiguration().AddInterceptor(MockInterceptor.Object);
                Catalog = new InterceptingCatalog(innerCatalog, cfg);
                Context();
            }

            public virtual void Context()
            {
            }
        }

        [TestFixture]
        public class when_querying_for_an_import : InterceptingCatalogPartsContext
        {
            [Test]
            public void it_should_return_non_intercepted_value_for_part1()
            {
                var part = Container.GetExportedValue<IPart>();
                Assert.That(part.GetType(), Is.EqualTo(typeof(Part1)));
            }

            [Test]
            public void it_should_return_an_intercepted_value_for_part2()
            {
                var part = Container.GetExportedValue<IPart>("part2");
                Assert.That(part.GetType(), Is.EqualTo(typeof(PartWrapper)));
                Assert.That(((PartWrapper)part).Inner.GetType(), Is.EqualTo(typeof(Part2)));
            }
        }

        public class InterceptingCatalogPartsContext
        {
            public CompositionContainer Container;

            public InterceptingCatalogPartsContext()
            {
                var innerCatalog = new TypeCatalog(typeof(Part1), typeof(Part2));
                var cfg = new InterceptionConfiguration()
                    .AddPartInterceptor(
                        new PredicateInterceptionCriteria(
                            new PartInterceptor(), d => d.Metadata.ContainsKey("metadata1")));
                var catalog = new InterceptingCatalog(innerCatalog, cfg);
                Container = new CompositionContainer(catalog);
                Context();
            }

            public virtual void Context()
            {
            }
        }

        public interface IPart {}

        [Export(typeof(IPart))]
        public class Part1 : IPart { }

        [Export("part2", typeof(IPart))]
        [PartMetadata("metadata1", true)]
        public class Part2 : IPart { }

        public class PartWrapper : IPart
        {
            public object Inner { get; set; }
        }

        public class PartInterceptor : IExportedValueInterceptor
        {
            public object Intercept(object value)
            {
                return new PartWrapper { Inner = value };
            }
        }
    }
}