using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using MefContrib.Integration.Exporters;
using MefContrib.Integration.Unity.Extensions;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace MefContrib.Integration.Unity.Tests
{
    [TestFixture]
    public class ContainerExportProviderTests
    {
        [Test]
        public void ExportProviderResolvesServiceRegisteredByTypeTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var adapter = new UnityContainerAdapter(unityContainer);
            var provider = new ContainerExportProvider(adapter);

            // Registration
            unityContainer.RegisterType<IUnityOnlyComponent, UnityOnlyComponent1>();

            var component = provider.GetExportedValue<IUnityOnlyComponent>();
            Assert.That(component, Is.Not.Null);
            Assert.That(component.GetType(), Is.EqualTo(typeof(UnityOnlyComponent1)));
        }

        [Test]
        public void ExportProviderResolvesServicesRegisteredByTypeTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var adapter = new UnityContainerAdapter(unityContainer);
            var provider = new ContainerExportProvider(adapter);

            // Registration
            unityContainer.RegisterType<IUnityOnlyComponent, UnityOnlyComponent1>();
            unityContainer.RegisterType<IUnityOnlyComponent, UnityOnlyComponent2>("b");

            var components = provider.GetExports<IUnityOnlyComponent>();
            Assert.That(components, Is.Not.Null);
            Assert.That(components.Count(), Is.EqualTo(2));

            Assert.That(components.Select(t => t.Value).OfType<UnityOnlyComponent1>().Count(), Is.EqualTo(1));
            Assert.That(components.Select(t => t.Value).OfType<UnityOnlyComponent2>().Count(), Is.EqualTo(1));
        }

        [Test]
        public void ExportProviderResolvesServiceRegisteredByTypeAndRegistrationNameTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var adapter = new UnityContainerAdapter(unityContainer);
            var provider = new ContainerExportProvider(adapter);

            // Registration
            unityContainer.RegisterType<IUnityOnlyComponent, UnityOnlyComponent2>("unityComponent2");

            var component = provider.GetExportedValue<IUnityOnlyComponent>("unityComponent2");
            Assert.That(component, Is.Not.Null);
            Assert.That(component.GetType(), Is.EqualTo(typeof(UnityOnlyComponent2)));
        }

        [Test]
        public void MefCanResolveLazyTypeRegisteredInUnityTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var adapter = new UnityContainerAdapter(unityContainer);
            var provider = new ContainerExportProvider(adapter);
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var container = new CompositionContainer(assemblyCatalog, provider);

            UnityOnlyComponent1.InstanceCount = 0;
            unityContainer.RegisterType<IUnityOnlyComponent, UnityOnlyComponent1>();

            var lazyUnityComponent = container.GetExport<IUnityOnlyComponent>();
            Assert.That(lazyUnityComponent, Is.Not.Null);
            Assert.That(UnityOnlyComponent1.InstanceCount, Is.EqualTo(0));

            Assert.That(lazyUnityComponent.Value, Is.Not.Null);
            Assert.That(lazyUnityComponent.Value.GetType(), Is.EqualTo(typeof(UnityOnlyComponent1)));
            Assert.That(UnityOnlyComponent1.InstanceCount, Is.EqualTo(1));
        }

        [Test]
        public void MefCanResolveLazyTypesRegisteredInUnityTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var adapter = new UnityContainerAdapter(unityContainer);
            var provider = new ContainerExportProvider(adapter);
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var container = new CompositionContainer(assemblyCatalog, provider);

            UnityOnlyComponent1.InstanceCount = 0;
            unityContainer.RegisterType<IUnityOnlyComponent, UnityOnlyComponent1>();
            unityContainer.RegisterType<IUnityOnlyComponent, UnityOnlyComponent2>("a");

            var lazyUnityComponent = container.GetExports<IUnityOnlyComponent>().ToList();
            Assert.That(lazyUnityComponent, Is.Not.Null);
            Assert.That(UnityOnlyComponent1.InstanceCount, Is.EqualTo(0));

            Assert.That(lazyUnityComponent, Is.Not.Null);
            Assert.That(lazyUnityComponent[0].Value, Is.Not.Null);
            Assert.That(lazyUnityComponent[1].Value, Is.Not.Null);
            Assert.That(UnityOnlyComponent1.InstanceCount, Is.EqualTo(1));
        }

        [Test]
        public void MefCannotResolveTypesRegisteredInUnityBeforeTrackingExtensionIsAddedTest()
        {
            // Setup
            var unityContainer = new UnityContainer();

            // Registration
            unityContainer.RegisterType<IUnityOnlyComponent, UnityOnlyComponent2>("unityComponent2");

            // Further setup
            var adapter = new UnityContainerAdapter(unityContainer);
            var provider = new ContainerExportProvider(adapter);
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var container = new CompositionContainer(assemblyCatalog, provider);

            Assert.That(delegate
            {
                container.GetExportedValue<IUnityOnlyComponent>("unityComponent2");
            }, Throws.TypeOf<ImportCardinalityMismatchException>());
        }

        [Test]
        public void MefCanResolveTypesRegisteredInUnityAfterTrackingExtensionIsAddedTest()
        {
            // Setup
            var unityContainer = new UnityContainer();

            // Enable tracking
            TypeRegistrationTrackerExtension.RegisterIfMissing(unityContainer);

            // Registration
            unityContainer.RegisterType<IUnityOnlyComponent, UnityOnlyComponent2>("unityComponent2");

            // Further setup
            var adapter = new UnityContainerAdapter(unityContainer);
            var provider = new ContainerExportProvider(adapter);
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var container = new CompositionContainer(assemblyCatalog, provider);

            var component = container.GetExportedValue<IUnityOnlyComponent>("unityComponent2");
            Assert.That(component, Is.Not.Null);
            Assert.That(component.GetType(), Is.EqualTo(typeof(UnityOnlyComponent2)));
        }

        [Test]
        public void CannotPassNullInstanceToTheContainerExportProviderConstructorTest()
        {
            Assert.That(delegate
            {
                new ContainerExportProvider(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void CannotPassNullUnityInstanceToTheUnityContainerAdapterConstructorTest()
        {
            Assert.That(delegate
            {
                new UnityContainerAdapter(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        #region Composing with two providers

        class A { }

        class B { }

        [Export]
        class C
        {
            [ImportingConstructor]
            public C(A a, B b)
            {
                ThingA = a;
                ThingB = b;
            }
            public A ThingA { get; private set; }

            public B ThingB { get; private set; }
        }

        [Test]
        public void ComposeWithTwoContainerExportProvidersTest()
        {
            var unityContainer1 = new UnityContainer();
            var exportProvider1 = new ContainerExportProvider(new UnityContainerAdapter(unityContainer1));

            var a = new A();
            unityContainer1.RegisterInstance<A>(a);

            var unityContainer2 = new UnityContainer();
            var exportProvider2 = new ContainerExportProvider(new UnityContainerAdapter(unityContainer2));

            var b = new B();
            unityContainer2.RegisterInstance<B>(b);

            var catalog = new TypeCatalog(typeof(C));
            var compositionContainer = new CompositionContainer(catalog, exportProvider1, exportProvider2);
            var instance = compositionContainer.GetExport<C>();
            Assert.IsNotNull(instance.Value);
            Assert.AreEqual(a, instance.Value.ThingA, "Instance of A is the same as that registered with the DI container.");
            Assert.AreEqual(b, instance.Value.ThingB, "Instance of B is the same as that registered with the DI container.");
        }

        #endregion
    }
}