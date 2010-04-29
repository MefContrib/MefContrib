using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using MefContrib.Integration.Unity.Exporters;
using MefContrib.Integration.Unity.Extensions;
using MefContrib.Integration.Unity.Tests.Helpers;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace MefContrib.Integration.Unity.Tests
{
    [TestFixture]
    public class UnityExportProviderTests
    {
        [Test]
        public void ExportProviderResolvesServiceRegisteredByTypeTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var provider = new UnityExportProvider(unityContainer);
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var container = new CompositionContainer(assemblyCatalog, provider);

            // Registration
            unityContainer.RegisterType<IUnityOnlyComponent, UnityOnlyComponent1>();

            var externalComponent = container.GetExportedValue<IUnityOnlyComponent>();
            Assert.That(externalComponent, Is.Not.Null);
            Assert.That(externalComponent.GetType(), Is.EqualTo(typeof(UnityOnlyComponent1)));
        }

        [Test]
        public void ExportProviderResolvesServiceRegisteredByTypeAndRegistrationNameTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var provider = new UnityExportProvider(unityContainer);
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var container = new CompositionContainer(assemblyCatalog, provider);

            // Registration
            unityContainer.RegisterType<IUnityOnlyComponent, UnityOnlyComponent2>("unityComponent2");

            var externalComponent = container.GetExportedValue<IUnityOnlyComponent>("unityComponent2");
            Assert.That(externalComponent, Is.Not.Null);
            Assert.That(externalComponent.GetType(), Is.EqualTo(typeof(UnityOnlyComponent2)));
        }

        [Test]
        public void MefCannotResolveTypesRegisteredInUnityBeforeTrackingExtensionIsAddedTest()
        {
            // Setup
            var unityContainer = new UnityContainer();

            // Registration
            unityContainer.RegisterType<IUnityOnlyComponent, UnityOnlyComponent2>("unityComponent2");

            // Further setup
            var provider = new UnityExportProvider(unityContainer);
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
            var provider = new UnityExportProvider(unityContainer);
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var container = new CompositionContainer(assemblyCatalog, provider);

            var externalComponent = container.GetExportedValue<IUnityOnlyComponent>("unityComponent2");
            Assert.That(externalComponent, Is.Not.Null);
            Assert.That(externalComponent.GetType(), Is.EqualTo(typeof(UnityOnlyComponent2)));
        }

        [Test]
        public void CannotPassNullUnityInstanceToTheConstructorTest()
        {
            Assert.That(delegate
            {
                new UnityExportProvider((IUnityContainer)null);
            }, Throws.TypeOf<ArgumentNullException>());

            Assert.That(delegate
            {
                new UnityExportProvider((Func<IUnityContainer>)null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void UnityContainerResolverCannotReturnNullInstanceTest()
        {
            var provider = new UnityExportProvider(TestUnityResolver);

            Assert.That(delegate
            {
                var unity = provider.UnityContainer;
            }, Throws.TypeOf<Exception>().And.Property("Message").EqualTo("Returned Unity instance is null."));
        }

        private static IUnityContainer TestUnityResolver()
        {
            return null;
        }
    }
}