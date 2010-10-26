using System;
using System.Collections.Generic;
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
    }

    public class PartFilter : IExportHandler
    {
        public void Initialize(ComposablePartCatalog interceptedCatalog)
        {
        }

        public IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition, IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> exports)
        {
            foreach (var export in exports)
            {
                if (export.Item1.Metadata.ContainsKey("ImportantPart") &&
                    export.Item1.Metadata["ImportantPart"].Equals(true))
                {
                    yield return export;
                }
            }
        }
    }

    public interface IPart
    {
        string Name { get; set; }
    }

    [Export(typeof(IPart))]
    [PartMetadata("ImportantPart", true)]
    public class Part0 : IPart
    {
        public string Name { get; set; }
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
            return new PartWrapper { Inner = (IPart)value };
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