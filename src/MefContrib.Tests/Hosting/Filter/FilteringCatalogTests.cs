using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using NUnit.Framework;

namespace MefContrib.Hosting.Filter.Tests
{
    [TestFixture]
    public class FilteringCatalogTests
    {
        public interface IMetadataComponent {}

        [Export(typeof(IMetadataComponent))]
        [PartMetadata("key", "value")]
        public class Component1
        {
        }

        [Export(typeof(IMetadataComponent))]
        public class Component2
        {
        }

        [Test]
        public void FilterBasedOnMetadataUsingContainsMetadataTest()
        {
            var catalog = new AssemblyCatalog(typeof(FilteringCatalogTests).Assembly);
            var filteredCatalog = new FilteringCatalog(catalog, new ContainsMetadata("key", "value"));
            var container = new CompositionContainer(filteredCatalog);
            var components = container.GetExports<IMetadataComponent>();

            Assert.That(components, Is.Not.Null);
            Assert.That(components.Count(), Is.EqualTo(1));
        }

        [Test]
        public void FilterBasedOnMetadataUsingLambdaExpressionTest()
        {
            var catalog = new AssemblyCatalog(typeof(FilteringCatalogTests).Assembly);
            var filteredCatalog = new FilteringCatalog(catalog,
                                                      partDefinition => partDefinition.Metadata.ContainsKey("key") &&
                                                                        partDefinition.Metadata["key"].Equals("value"));
            var container = new CompositionContainer(filteredCatalog);
            var components = container.GetExports<IMetadataComponent>();

            Assert.That(components, Is.Not.Null);
            Assert.That(components.Count(), Is.EqualTo(1));
        }

        public interface ILifetimeComponent
        {
        }

        [Export(typeof(ILifetimeComponent))]
        public class LifetimeComponent1 : ILifetimeComponent
        {
        }

        [Export(typeof(ILifetimeComponent))]
        [PartCreationPolicy(CreationPolicy.Shared)]
        public class LifetimeComponent2 : ILifetimeComponent
        {
        }

        [Export(typeof(ILifetimeComponent))]
        [PartCreationPolicy(CreationPolicy.NonShared)]
        public class LifetimeComponent3 : ILifetimeComponent
        {
        }

        [Export(typeof(ILifetimeComponent))]
        [PartCreationPolicy(CreationPolicy.Any)]
        public class LifetimeComponent4 : ILifetimeComponent
        {
        }

        [Test]
        public void FilterBasedOnSharedLifetimeUsingHasCreationPolicyTest()
        {
            var catalog = new AssemblyCatalog(typeof(FilteringCatalogTests).Assembly);
            var filteredCatalog = new FilteringCatalog(catalog, new HasCreationPolicy(CreationPolicy.Shared));
            var container = new CompositionContainer(filteredCatalog);
            var components = container.GetExports<ILifetimeComponent>();

            Assert.That(components, Is.Not.Null);
            Assert.That(components.Count(), Is.EqualTo(1));
            Assert.That(components.First().Value.GetType(), Is.EqualTo(typeof(LifetimeComponent2)));
        }

        [Test]
        public void FilterBasedOnNonSharedLifetimeUsingHasCreationPolicyTest()
        {
            var catalog = new AssemblyCatalog(typeof(FilteringCatalogTests).Assembly);
            var filteredCatalog = new FilteringCatalog(catalog, new HasCreationPolicy(CreationPolicy.NonShared));
            var container = new CompositionContainer(filteredCatalog);
            var components = container.GetExports<ILifetimeComponent>();

            Assert.That(components, Is.Not.Null);
            Assert.That(components.Count(), Is.EqualTo(1));
            Assert.That(components.First().Value.GetType(), Is.EqualTo(typeof(LifetimeComponent3)));
        }

        [Test]
        public void FilterBasedOnAnyLifetimeUsingHasCreationPolicyTest()
        {
            var catalog = new AssemblyCatalog(typeof(FilteringCatalogTests).Assembly);
            var filteredCatalog = new FilteringCatalog(catalog, new HasCreationPolicy(CreationPolicy.Any));
            var container = new CompositionContainer(filteredCatalog);
            var components = container.GetExports<ILifetimeComponent>();

            Assert.That(components, Is.Not.Null);
            Assert.That(components.Count(), Is.EqualTo(2));
            Assert.That(components.Select(t => t.Value).OfType<LifetimeComponent1>().First().GetType(), Is.EqualTo(typeof(LifetimeComponent1)));
            Assert.That(components.Select(t => t.Value).OfType<LifetimeComponent4>().First().GetType(), Is.EqualTo(typeof(LifetimeComponent4)));
        }

        [Export]
        [PartCreationPolicy(CreationPolicy.NonShared)]
        public class Root : IDisposable
        {
            [Import]
            public Dep1 Dep { get; set; }

            public void Dispose()
            {
                Disposed = true;
            }

            public bool Disposed { get; private set; }
        }

        [Export]
        [PartCreationPolicy(CreationPolicy.NonShared)]
        public class Dep1 : IDisposable
        {
            [Import]
            public Dep2 Dep { get; set; }

            public void Dispose()
            {
                Disposed = true;
            }

            public bool Disposed { get; private set; }
        }

        [Export]
        [PartCreationPolicy(CreationPolicy.Shared)]
        public class Dep2 : IDisposable
        {
            [Import]
            public Dep3 Dep { get; set; }

            public void Dispose()
            {
                Disposed = true;
            }

            public bool Disposed { get; private set; }
        }

        [Export]
        [PartCreationPolicy(CreationPolicy.NonShared)]
        public class Dep3 : IDisposable
        {
            public void Dispose()
            {
                Disposed = true;
            }

            public bool Disposed { get; private set; }
        }

        [Test]
        public void ParentChildContainerTest()
        {
            var catalog = new AssemblyCatalog(typeof(FilteringCatalogTests).Assembly);
            var parent = new CompositionContainer(catalog);

            var filteredCat = new FilteringCatalog(catalog, new HasCreationPolicy(CreationPolicy.NonShared));
            var child = new CompositionContainer(filteredCat, parent);

            var root = child.GetExportedValue<Root>();
            var dep1 = root.Dep;
            var dep2 = dep1.Dep;
            var dep3 = dep2.Dep;

            Assert.That(root.Disposed, Is.False);
            Assert.That(dep1.Disposed, Is.False);
            Assert.That(dep2.Disposed, Is.False);
            Assert.That(dep3.Disposed, Is.False);
            
            child.Dispose();

            Assert.That(root.Disposed, Is.True); // Disposed as it was created by the child container
            Assert.That(dep1.Disposed, Is.True); // Disposed as it was created by the child container
            Assert.That(dep2.Disposed, Is.False);
            Assert.That(dep3.Disposed, Is.False);
        }
    }
}