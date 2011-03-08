using System.ComponentModel.Composition.Hosting;
using System.Linq;
using MefContrib.Hosting.Interception.Configuration;
using NUnit.Framework;

namespace MefContrib.Hosting.Interception.Tests
{
    [TestFixture]
    public class RecompositionTests
    {
        private CompositionContainer container;

        [Test]
        public void When_adding_new_part_to_the_intercepted_catalog_intercepting_catalog_is_recomposed_and_intercepts_that_part()
        {
            var innerCatalog = new TypeCatalog(typeof(RecomposablePart1), typeof(RecomposablePartImporter));
            var cfg = new InterceptionConfiguration().AddInterceptor(new RecomposablePartInterceptor());
            var aggregateCatalog = new AggregateCatalog(innerCatalog);
            var catalog = new InterceptingCatalog(aggregateCatalog, cfg);
            container = new CompositionContainer(catalog);

            var importer = container.GetExportedValue<RecomposablePartImporter>();
            Assert.That(importer, Is.Not.Null);
            Assert.That(importer.Parts, Is.Not.Null);
            Assert.That(importer.Parts.Length, Is.EqualTo(1));
            Assert.That(importer.Parts[0].Count, Is.EqualTo(1));
            Assert.That(catalog.Parts.Count(), Is.EqualTo(2));

            // Recompose
            aggregateCatalog.Catalogs.Add(new TypeCatalog(typeof(RecomposablePart2)));

            Assert.That(importer, Is.Not.Null);
            Assert.That(importer.Parts, Is.Not.Null);
            Assert.That(importer.Parts.Length, Is.EqualTo(2));
            Assert.That(importer.Parts[0].Count, Is.EqualTo(1));
            Assert.That(importer.Parts[1].Count, Is.EqualTo(1));
            Assert.That(catalog.Parts.Count(), Is.EqualTo(3));
            Assert.That(catalog.Parts.OfType<InterceptingComposablePartDefinition>().Count(), Is.EqualTo(3));
        }

        [Test]
        public void When_adding_new_part_to_the_intercepted_catalog_intercepting_catalog_raises_recomposition_events()
        {
            var innerCatalog = new TypeCatalog(typeof(RecomposablePart1));
            var cfg = new InterceptionConfiguration().AddInterceptor(new RecomposablePartInterceptor());
            var aggregateCatalog = new AggregateCatalog(innerCatalog);
            var catalog = new InterceptingCatalog(aggregateCatalog, cfg);
            container = new CompositionContainer(catalog);
            
            uint changingEventInvokeCount = 0;
            uint changedEventInvokeCount = 0;

            catalog.Changing += (s, e) => changingEventInvokeCount++;
            catalog.Changed += (s, e) => changedEventInvokeCount++;
            
            // Recompose
            aggregateCatalog.Catalogs.Add(new TypeCatalog(typeof(RecomposablePart2)));

            Assert.That(changingEventInvokeCount, Is.EqualTo(1));
            Assert.That(changedEventInvokeCount, Is.EqualTo(1));
        }

        [Test]
        public void When_removing_a_part_from_the_intercepted_catalog_intercepting_catalog_is_recomposed_and_removes_that_part()
        {
            var innerCatalog1 = new TypeCatalog(typeof(RecomposablePart1), typeof(RecomposablePartImporter));
            var innerCatalog2 = new TypeCatalog(typeof(RecomposablePart2));
            var cfg = new InterceptionConfiguration().AddInterceptor(new RecomposablePartInterceptor());
            var aggregateCatalog = new AggregateCatalog(innerCatalog1, innerCatalog2);
            var catalog = new InterceptingCatalog(aggregateCatalog, cfg);
            container = new CompositionContainer(catalog);

            var importer = container.GetExportedValue<RecomposablePartImporter>();
            Assert.That(importer, Is.Not.Null);
            Assert.That(importer.Parts, Is.Not.Null);
            Assert.That(importer.Parts.Length, Is.EqualTo(2));
            Assert.That(importer.Parts[0].Count, Is.EqualTo(1));
            Assert.That(importer.Parts[1].Count, Is.EqualTo(1));
            Assert.That(catalog.Parts.Count(), Is.EqualTo(3));

            // Recompose
            aggregateCatalog.Catalogs.Remove(innerCatalog2);

            Assert.That(importer, Is.Not.Null);
            Assert.That(importer.Parts, Is.Not.Null);
            Assert.That(importer.Parts.Length, Is.EqualTo(1));
            Assert.That(importer.Parts[0].Count, Is.EqualTo(1));
            Assert.That(catalog.Parts.Count(), Is.EqualTo(2));
        }

        [Test]
        public void When_removing_part_from_the_intercepted_catalog_intercepting_catalog_raises_recomposition_events()
        {
            var innerCatalog1 = new TypeCatalog(typeof(RecomposablePart1));
            var innerCatalog2 = new TypeCatalog(typeof(RecomposablePart2));
            var cfg = new InterceptionConfiguration().AddInterceptor(new RecomposablePartInterceptor());
            var aggregateCatalog = new AggregateCatalog(innerCatalog1, innerCatalog2);
            var catalog = new InterceptingCatalog(aggregateCatalog, cfg);
            container = new CompositionContainer(catalog);

            uint changingEventInvokeCount = 0;
            uint changedEventInvokeCount = 0;

            catalog.Changing += (s, e) => changingEventInvokeCount++;
            catalog.Changed += (s, e) => changedEventInvokeCount++;

            // Recompose
            aggregateCatalog.Catalogs.Remove(innerCatalog2);

            Assert.That(changingEventInvokeCount, Is.EqualTo(1));
            Assert.That(changedEventInvokeCount, Is.EqualTo(1));
        }
    }
}