using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace MefContrib.Integration.Unity.Tests
{
    [TestFixture]
    public class BidirectionalIntegrationTests
    {
        [Test]
        public void UnityCanResolveMefComponentThatHasUnityDependenciesTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Register catalog and types
            unityContainer.RegisterCatalog(assemblyCatalog);
            unityContainer.RegisterType<IUnityOnlyComponent, UnityOnlyComponent1>();

            // Test
            var mefComponent = unityContainer.Resolve<IMefComponentWithUnityDependencies>();
            Assert.That(mefComponent, Is.Not.Null);
            Assert.That(mefComponent.MefOnlyComponent.GetType(), Is.EqualTo(typeof(MefComponent1)));
            Assert.That(mefComponent.UnityOnlyComponent.GetType(), Is.EqualTo(typeof(UnityOnlyComponent1)));
        }

        [Test]
        public void UnityCanResolveMefComponentThatHasUnityDependenciesThatHaveMefDependenciesTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Register catalog and types
            unityContainer.RegisterCatalog(assemblyCatalog);
            unityContainer.RegisterType<IUnityOnlyComponent, UnityOnlyComponent1>();
            unityContainer.RegisterType<IUnityComponent, UnityComponent1>();

            // Test
            var mefComponent = unityContainer.Resolve<IMefComponentWithUnityDependencies>("component2");
            Assert.That(mefComponent, Is.Not.Null);
            Assert.That(mefComponent.GetType(), Is.EqualTo(typeof(MefComponentWithUnityDependencies2)));
            Assert.That(mefComponent.MefOnlyComponent.GetType(), Is.EqualTo(typeof(MefComponent1)));
            Assert.That(mefComponent.UnityOnlyComponent.GetType(), Is.EqualTo(typeof(UnityOnlyComponent1)));

            var mefComponentWithUnityDependencies2 = (MefComponentWithUnityDependencies2) mefComponent;
            Assert.That(mefComponentWithUnityDependencies2.MixedUnityMefComponent.GetType(), Is.EqualTo(typeof(UnityComponent1)));
            Assert.That(mefComponentWithUnityDependencies2.MixedUnityMefComponent.MefComponent.GetType(), Is.EqualTo(typeof(MefComponent1)));
        }

        [Test]
        public void UnityCircularDependencyIsDetectedTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Register catalog and types
            unityContainer.RegisterCatalog(assemblyCatalog);
            unityContainer.RegisterType<UnityOnlyComponent1>();
            
            // Test
            var unityOnlyComponent1 = unityContainer.Resolve<UnityOnlyComponent1>();
            Assert.That(unityOnlyComponent1, Is.Not.Null);
        }

        [Test]
        public void UnityCanResolveUnityComponentThatHasUnityAndMefDependenciesTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Register catalog and types
            unityContainer.RegisterCatalog(assemblyCatalog);
            unityContainer.RegisterType<IUnityOnlyComponent, UnityOnlyComponent1>();
            unityContainer.RegisterType<UnityMixedComponent>();

            // Test
            var unityMixedComponent = unityContainer.Resolve<UnityMixedComponent>();
            Assert.That(unityMixedComponent, Is.Not.Null);
            Assert.That(unityMixedComponent.GetType(), Is.EqualTo(typeof(UnityMixedComponent)));
            Assert.That(unityMixedComponent.MefComponent.GetType(), Is.EqualTo(typeof(MefComponent1)));
            Assert.That(unityMixedComponent.UnityComponent.GetType(), Is.EqualTo(typeof(UnityOnlyComponent1)));
        }

        [Test]
        public void UnityContainerCanBeResolvedByMefTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Register catalog and types
            unityContainer.RegisterCatalog(assemblyCatalog);

            var compositionContainer1 = unityContainer.Resolve<CompositionContainer>();
            var compositionContainer2 = unityContainer.Resolve<CompositionContainer>();
            Assert.That(compositionContainer1, Is.Not.Null);
            Assert.That(compositionContainer2, Is.Not.Null);
            Assert.That(compositionContainer1, Is.SameAs(compositionContainer2));

            var unityContainerFromMef1 = compositionContainer1.GetExportedValue<IUnityContainer>();
            var unityContainerFromMef2 = compositionContainer1.GetExportedValue<IUnityContainer>();
            
            Assert.That(unityContainerFromMef1, Is.Not.Null);
            Assert.That(unityContainerFromMef2, Is.Not.Null);
            Assert.AreSame(unityContainerFromMef1, unityContainerFromMef2);
            Assert.AreSame(unityContainer, unityContainerFromMef1);
        }

        [Test]
        public void MefResolvesServiceRegisteredInUnityByTypeTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Register catalog and types
            unityContainer.RegisterCatalog(assemblyCatalog);

            // Registration
            unityContainer.RegisterType<IUnityOnlyComponent, UnityOnlyComponent1>(new ContainerControlledLifetimeManager());

            var container = unityContainer.Resolve<CompositionContainer>();
            var unityOnlyComponent = container.GetExportedValue<IUnityOnlyComponent>();
            var unityOnlyComponent2 = unityContainer.Resolve<IUnityOnlyComponent>();
            Assert.That(unityOnlyComponent, Is.Not.Null);
            Assert.That(unityOnlyComponent.GetType(), Is.EqualTo(typeof(UnityOnlyComponent1)));
            Assert.That(unityOnlyComponent2, Is.Not.Null);
            Assert.That(unityOnlyComponent2.GetType(), Is.EqualTo(typeof(UnityOnlyComponent1)));
            Assert.That(unityOnlyComponent, Is.EqualTo(unityOnlyComponent2));
        }

        [Test]
        public void MefCanResolveMefComponentThatHasUnityAndMefDependenciesTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Register catalog and types
            unityContainer.RegisterCatalog(assemblyCatalog);
            unityContainer.RegisterType<IUnityOnlyComponent, UnityOnlyComponent1>();

            // Test
            var container = unityContainer.Resolve<CompositionContainer>();
            var mefMixedComponent = container.GetExportedValue<MefMixedComponent>();
            Assert.That(mefMixedComponent, Is.Not.Null);
            Assert.That(mefMixedComponent.GetType(), Is.EqualTo(typeof(MefMixedComponent)));
            Assert.That(mefMixedComponent.MefComponent.GetType(), Is.EqualTo(typeof(MefComponent1)));
            Assert.That(mefMixedComponent.UnityComponent.GetType(), Is.EqualTo(typeof(UnityOnlyComponent1)));
        }

        [Test]
        public void UnityResolvesUnityComponentRegisteredWithoutInterfaceTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Register catalog and types
            unityContainer.RegisterCatalog(assemblyCatalog);

            // Registration
            unityContainer.RegisterType<UnityComponent3>();

            var component2 = unityContainer.Resolve<UnityComponent2>();
            Assert.That(component2, Is.Not.Null);
            Assert.That(component2.ImportedMefComponent, Is.Not.Null);
            Assert.That(component2.ImportedMefComponent.GetType(), Is.EqualTo(typeof(MefComponent2)));
            Assert.That(component2.MefComponent.GetType(), Is.EqualTo(typeof(MefComponent2)));
        }

        public class MefSingletonComponent
        {
            public static int Counter;

            public MefSingletonComponent()
            {
                Counter++;
            }
        }

        [Test]
        public void UnityCanResolveMefComponentRegisteredUsingAddExportedValueTest()
        {
            MefSingletonComponent.Counter = 0;

            // Setup
            var unityContainer = new UnityContainer();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Register catalog and types
            unityContainer.RegisterCatalog(assemblyCatalog);
            var compositionContainer = unityContainer.Configure<CompositionIntegration>().CompositionContainer;
            var batch = new CompositionBatch();
            var singletonComponent = new MefSingletonComponent();

            batch.AddExportedValue(singletonComponent);
            compositionContainer.Compose(batch);

            var singletonComponent1 = compositionContainer.GetExport<MefSingletonComponent>().Value;
            Assert.That(MefSingletonComponent.Counter, Is.EqualTo(1));
            Assert.That(singletonComponent1, Is.SameAs(singletonComponent));

            var singletonComponent2 = unityContainer.Resolve<MefSingletonComponent>();
            Assert.That(MefSingletonComponent.Counter, Is.EqualTo(1));
            Assert.That(singletonComponent2, Is.SameAs(singletonComponent));
        }
    }
}