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

            [Test]
            public void it_should_return_a_part_with_properly_set_name()
            {
                var part1 = Container.GetExportedValue<IPart>();
                var part2 = Container.GetExportedValue<IPart>("part2");

                Assert.That(part1.Name, Is.EqualTo("Name property is set be the interceptor."));
                Assert.That(part2.Name, Is.EqualTo("Name property is set be the interceptor."));
            }

            [Test]
            public void it_should_return_a_part_with_respect_to_its_creation_policy()
            {
                Part1.InstanceCount = 0;
                Part3.InstanceCount = 0;

                var part11 = Container.GetExportedValue<IPart>();
                var part12 = Container.GetExportedValue<IPart>();
                var part31 = Container.GetExportedValue<IPart>("part3");
                var part32 = Container.GetExportedValue<IPart>("part3");

                Assert.That(Part1.InstanceCount, Is.EqualTo(1));
                Assert.That(Part3.InstanceCount, Is.EqualTo(2));
                Assert.That(part11, Is.SameAs(part12));
                Assert.That(part31, Is.Not.SameAs(part32));
            }
        }

        public class InterceptingCatalogPartsContext
        {
            public CompositionContainer Container;

            public InterceptingCatalogPartsContext()
            {
                var innerCatalog = new TypeCatalog(typeof(Part1), typeof(Part2), typeof(Part3));
                var cfg = new InterceptionConfiguration()
                    .AddInterceptor(new GeneralInterceptor())
                    .AddInterceptionCriteria(
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

        public interface IPart
        {
            string Name { get; set; }
        }

        [Export(typeof(IPart))]
        [PartCreationPolicy(CreationPolicy.Shared)]
        public class Part1 : IPart
        {
            public static int InstanceCount;
            
            public Part1()
            {
                InstanceCount++;
            }

            public string Name { get; set; }
        }

        [Export("part2", typeof(IPart))]
        [PartMetadata("metadata1", true)]
        public class Part2 : IPart
        {
            public string Name { get; set; }
        }

        [Export("part3", typeof(IPart))]
        [PartCreationPolicy(CreationPolicy.NonShared)]
        public class Part3 : IPart
        {
            public static int InstanceCount;
            
            public Part3()
            {
                InstanceCount++;
            }

            public string Name { get; set; }
        }

        public class PartWrapper : IPart
        {
            public string Name
            {
                get { return Inner.Name; }
                set { Inner.Name = value; }
            }

            public IPart Inner { get; set; }
        }

        public class PartInterceptor : IExportedValueInterceptor
        {
            public object Intercept(object value)
            {
                return new PartWrapper { Inner = (IPart) value };
            }
        }

        public class GeneralInterceptor : IExportedValueInterceptor
        {
            public object Intercept(object value)
            {
                var part = value as IPart;
                if (part != null)
                {
                    part.Name = "Name property is set be the interceptor.";
                }

                return value;
            }
        }
    }
}