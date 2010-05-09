using System;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace MefContrib.Integration.Unity.Tests
{
    [TestFixture]
    public class CompositionIntegrationTests
    {
        [Test]
        public void UnityCanResolveMefComponentRegisteredByTypeTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Add composition support for unity
            unityContainer.AddNewExtension<CompositionIntegration>();
            unityContainer.Configure<CompositionIntegration>().Catalogs.Add(assemblyCatalog);

            var mefComponent = unityContainer.Resolve<IMefComponent>();
            Assert.That(mefComponent, Is.Not.Null);
            Assert.That(mefComponent.GetType(), Is.EqualTo(typeof(MefComponent1)));

            unityContainer.RegisterType<IUnityComponent, UnityComponent1>();

            var unityComponent = unityContainer.Resolve<IUnityComponent>();
            Assert.That(unityComponent, Is.Not.Null);
            Assert.That(unityComponent.MefComponent.GetType(), Is.EqualTo(typeof(MefComponent1)));
        }

        [Test]
        public void UnityCanResolveMefComponentRegisteredByTypeAndRegistrationNameTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Add composition support for unity
            unityContainer.AddNewExtension<CompositionIntegration>();
            unityContainer.Configure<CompositionIntegration>().Catalogs.Add(assemblyCatalog);

            var mefComponent = unityContainer.Resolve<IMefComponent>("component2");
            Assert.That(mefComponent, Is.Not.Null);
            Assert.That(mefComponent.GetType(), Is.EqualTo(typeof(MefComponent2)));

            unityContainer.RegisterType<IUnityComponent, UnityComponent2>();

            var unityComponent = unityContainer.Resolve<IUnityComponent>();
            Assert.That(unityComponent, Is.Not.Null);
            Assert.That(unityComponent.MefComponent.GetType(), Is.EqualTo(typeof(MefComponent2)));
        }

        [Test]
        public void UnityCanResolveMefComponentRegisteredByTypeUsingConstructorInjectionTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Add composition support for unity
            unityContainer.AddNewExtension<CompositionIntegration>();
            unityContainer.Configure<CompositionIntegration>().Catalogs.Add(assemblyCatalog);

            unityContainer.RegisterType<IUnityComponent, UnityComponent1>();

            var unityComponent = unityContainer.Resolve<IUnityComponent>();
            Assert.That(unityComponent, Is.Not.Null);
            Assert.That(unityComponent.MefComponent.GetType(), Is.EqualTo(typeof(MefComponent1)));
        }

        [Test]
        public void UnityCanResolveMefComponentRegisteredByTypeAndRegistrationNameUsingConstructorInjectionTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Add composition support for unity
            unityContainer.AddNewExtension<CompositionIntegration>();
            unityContainer.Configure<CompositionIntegration>().Catalogs.Add(assemblyCatalog);

            unityContainer.RegisterType<IUnityComponent, UnityComponent2>();

            var unityComponent = unityContainer.Resolve<IUnityComponent>();
            Assert.That(unityComponent, Is.Not.Null);
            Assert.That(unityComponent.MefComponent.GetType(), Is.EqualTo(typeof(MefComponent2)));
        }

        [Test]
        public void UnitySatisfiesMefImportsByTypeOnUnityComponentsTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Add composition support for unity
            unityContainer.AddNewExtension<CompositionIntegration>();
            unityContainer.Configure<CompositionIntegration>().Catalogs.Add(assemblyCatalog);

            unityContainer.RegisterType<IUnityComponent, UnityComponent1>();

            var unityComponent = unityContainer.Resolve<IUnityComponent>();
            Assert.That(unityComponent, Is.Not.Null);
            Assert.That(unityComponent.ImportedMefComponent.GetType(), Is.EqualTo(typeof(MefComponent1)));
        }

        [Test]
        public void UnityLazySatisfiesMefImportsByTypeOnUnityComponentsTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Add composition support for unity
            unityContainer.AddNewExtension<CompositionIntegration>();
            unityContainer.Configure<CompositionIntegration>().Catalogs.Add(assemblyCatalog);

            unityContainer.RegisterType<IUnityComponent, UnityComponent11>();

            var unityComponent = unityContainer.Resolve<IUnityComponent>();
            Assert.That(unityComponent, Is.Not.Null);
            Assert.That(unityComponent.GetType(), Is.EqualTo(typeof(UnityComponent11)));
            Assert.That(unityComponent.ImportedMefComponent.GetType(), Is.EqualTo(typeof(MefComponent1)));
            Assert.That(unityComponent.MefComponent.GetType(), Is.EqualTo(typeof(MefComponent1)));

            var unityComponent11 = (UnityComponent11) unityComponent;
            var mefComponent = unityComponent11.MefComponentFactory();
            Assert.That(mefComponent, Is.SameAs(unityComponent.MefComponent));
        }

        [Test]
        public void UnitySatisfiesMefImportsByTypeAndRegistrationNameOnUnityComponentsTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Add composition support for unity
            unityContainer.AddNewExtension<CompositionIntegration>();
            unityContainer.Configure<CompositionIntegration>().Catalogs.Add(assemblyCatalog);

            unityContainer.RegisterType<IUnityComponent, UnityComponent2>();

            var unityComponent = unityContainer.Resolve<IUnityComponent>();
            Assert.That(unityComponent, Is.Not.Null);
            Assert.That(unityComponent.ImportedMefComponent.GetType(), Is.EqualTo(typeof(MefComponent2)));
        }

        [Test]
        public void UnityDoesNotSatisfyMefImportsOnUnityComponentsWhenMarkedWithPartNotComposableAttributeTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Add composition support for unity
            unityContainer.AddNewExtension<CompositionIntegration>();
            unityContainer.Configure<CompositionIntegration>().Catalogs.Add(assemblyCatalog);

            unityContainer.RegisterType<IUnityComponent, UnityComponent3>();

            var unityComponent = unityContainer.Resolve<IUnityComponent>();
            Assert.That(unityComponent, Is.Not.Null);
            Assert.That(unityComponent.ImportedMefComponent, Is.Null);
        }

        [Test]
        public void UnityCanResolveCompositionContainerTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Add composition support for unity
            unityContainer.AddNewExtension<CompositionIntegration>();
            unityContainer.Configure<CompositionIntegration>().Catalogs.Add(assemblyCatalog);

            var compositionContainer = unityContainer.Resolve<CompositionContainer>();
            Assert.That(compositionContainer, Is.Not.Null);
        }

        [Test]
        public void UnityCannotResolveCompositionContainerWhenExplicitlyDisallowedTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Add composition support for unity
            unityContainer.AddExtension(new CompositionIntegration(false));
            unityContainer.Configure<CompositionIntegration>().Catalogs.Add(assemblyCatalog);

            var internalCompositionContainer = unityContainer.Configure<CompositionIntegration>().CompositionContainer;
            Assert.That(internalCompositionContainer, Is.Not.Null);
            Assert.That(unityContainer.Configure<CompositionIntegration>().Register, Is.False);

            Assert.That(delegate
            {
                unityContainer.Resolve<CompositionContainer>();
            }, Throws.TypeOf<ResolutionFailedException>());
        }

        [Test]
        public void UnityCannotResolveMultipleMefInstancesTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Add composition support for unity
            unityContainer.AddNewExtension<CompositionIntegration>();
            unityContainer.Configure<CompositionIntegration>().Catalogs.Add(assemblyCatalog);

            Assert.That(delegate
            {
                unityContainer.Resolve<IMultipleMefComponent>();
            }, Throws.TypeOf<ResolutionFailedException>());
        }

        [Test]
        public void DisposingUnityDisposesCompositionContainerTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Add composition support for unity
            unityContainer.AddNewExtension<CompositionIntegration>();
            unityContainer.Configure<CompositionIntegration>().Catalogs.Add(assemblyCatalog);

            var compositionContainer = unityContainer.Resolve<CompositionContainer>();
            unityContainer.Dispose();
            
            Assert.That(delegate
            {
                compositionContainer.GetExport<IMefComponent>();
            }, Throws.TypeOf<ObjectDisposedException>());
        }
    }
}