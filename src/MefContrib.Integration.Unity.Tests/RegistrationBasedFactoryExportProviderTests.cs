using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using MefContrib.Integration.Exporters;
using NUnit.Framework;

namespace MefContrib.Integration.Unity.Tests
{
    [TestFixture]
    public class RegistrationBasedFactoryExportProviderTests
    {
        #region Fake External Components

        public interface IExternalComponent
        {
            void Foo();
        }

        public class ExternalComponent1 : IExternalComponent
        {
            public void Foo()
            {
            }
        }

        public class ExternalComponent2 : IExternalComponent
        {
            public void Foo()
            {
            }
        }

        #endregion

        #region Fake MEF Components

        public interface IMefComponent
        {
            void Foo();

            IExternalComponent Component1 { get; }

            IExternalComponent Component1A { get; set; }
        }

        [Export(typeof(IMefComponent))]
        [PartCreationPolicy(CreationPolicy.Shared)]
        public class MefComponent1 : IMefComponent
        {
            private readonly IExternalComponent m_Component1;

            [ImportingConstructor]
            public MefComponent1(IExternalComponent component1)
            {
                m_Component1 = component1;
            }

            public void Foo()
            {
            }

            public IExternalComponent Component1
            {
                get { return m_Component1; }
            }

            [Import]
            public IExternalComponent Component1A { get; set; }
        }

        [Export("component2", typeof(IMefComponent))]
        [PartCreationPolicy(CreationPolicy.Shared)]
        public class MefComponent2 : IMefComponent
        {
            private readonly IExternalComponent m_Component1;

            [ImportingConstructor]
            public MefComponent2([Import("external2")] IExternalComponent component1)
            {
                m_Component1 = component1;
            }

            public void Foo()
            {
            }

            public IExternalComponent Component1
            {
                get { return m_Component1; }
            }

            [Import("external2")]
            public IExternalComponent Component1A { get; set; }
        }

        #endregion

        [Test]
        public void ExportProviderResolvesServiceRegisteredByTypeTest()
        {
            // Setup
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var provider = new RegistrationBasedFactoryExportProvider(FactoryMethod1);
            var container = new CompositionContainer(assemblyCatalog, provider);

            // Registration
            provider.Register(typeof(IExternalComponent));

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
            var provider = new RegistrationBasedFactoryExportProvider(FactoryMethod1);
            var container = new CompositionContainer(assemblyCatalog, provider);

            // Registration
            provider.Register(typeof(IExternalComponent), "external2");

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