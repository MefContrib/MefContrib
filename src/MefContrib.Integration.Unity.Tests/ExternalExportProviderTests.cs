using System;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using MefContrib.Integration.Unity.Exporters;
using MefContrib.Integration.Unity.Tests.Helpers.External;
using NUnit.Framework;

namespace MefContrib.Integration.Unity.Tests
{
    [TestFixture]
    public class ExternalExportProviderTests
    {
        [Test]
        public void ExportProviderResolvesServiceRegisteredByTypeTest()
        {
            // Setup
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var provider = new ExternalExportProvider(FactoryMethod1);
            var container = new CompositionContainer(assemblyCatalog, provider);

            // Registration
            provider.AddExportDefinition(typeof(IExternalComponent));

            var externalComponent = container.GetExportedValue<IExternalComponent>();
            Assert.That(externalComponent, Is.Not.Null);
            Assert.That(externalComponent.GetType(), Is.EqualTo(typeof(ExternalComponent1)));

            var mefComponent = container.GetExportedValue<IMefComponent>();
            Assert.That(mefComponent, Is.Not.Null);
            Assert.That(mefComponent.Component1.GetType(), Is.EqualTo(typeof(ExternalComponent1)));
            Assert.That(mefComponent.Component1A.GetType(), Is.EqualTo(typeof(ExternalComponent1)));
        }

        [Test]
        public void ExportProviderResolvesServiceRegisteredByTypeAndRegistrationNameTest()
        {
            // Setup
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var provider = new ExternalExportProvider(FactoryMethod1);
            var container = new CompositionContainer(assemblyCatalog, provider);

            // Registration
            provider.AddExportDefinition(typeof(IExternalComponent), "external2");

            var externalComponent = container.GetExportedValue<IExternalComponent>("external2");
            Assert.That(externalComponent, Is.Not.Null);
            Assert.That(externalComponent.GetType(), Is.EqualTo(typeof(ExternalComponent2)));

            var mefComponent = container.GetExportedValue<IMefComponent>("component2");
            Assert.That(mefComponent, Is.Not.Null);
            Assert.That(mefComponent.Component1.GetType(), Is.EqualTo(typeof(ExternalComponent2)));
            Assert.That(mefComponent.Component1A.GetType(), Is.EqualTo(typeof(ExternalComponent2)));
        }

        private static object FactoryMethod1(Type type, string registrationName)
        {
            if (type == typeof(IExternalComponent) && registrationName == null)
                return new ExternalComponent1();

            if (type == typeof(IExternalComponent) && registrationName == "external2")
                return new ExternalComponent2();

            return null;
        }
    }
}