using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using MefContrib.Hosting;
using NUnit.Framework;

namespace MefContrib.Tests.Hosting
{
    [TestFixture]
    public class FactoryExportProviderTests
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

            IExternalComponent Component2 { get; }
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
            public IExternalComponent Component2 { get; set; }
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
            public IExternalComponent Component2 { get; set; }
        }

        [Export("component3", typeof(IMefComponent))]
        [PartCreationPolicy(CreationPolicy.Shared)]
        public class MefComponent3 : IMefComponent
        {
            public void Foo()
            {
            }

            [Import]
            public IExternalComponent Component1 { get; set; }

            [Import("external2")]
            public IExternalComponent Component2 { get; set; }
        }

        [Export("component4", typeof(IMefComponent))]
        [PartCreationPolicy(CreationPolicy.Shared)]
        public class MefComponent4 : IMefComponent
        {
            [ImportingConstructor]
            public MefComponent4(IExternalComponent component1, [Import("external2")] IExternalComponent component2)
            {
                Component1 = component1;
                Component2 = component2;
            }

            public void Foo()
            {
            }

            public IExternalComponent Component1 { get; set; }
            
            public IExternalComponent Component2 { get; set; }
        }

        #endregion

        [Test]
        public void ExportProviderResolvesServiceRegisteredByTypeTest()
        {
            // Setup
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var provider = new FactoryExportProvider(FactoryMethod1);
            var container = new CompositionContainer(assemblyCatalog, provider);

            // Registration
            provider.Register(typeof(IExternalComponent));

            var externalComponent = container.GetExportedValue<IExternalComponent>();
            Assert.That(externalComponent, Is.Not.Null);
            Assert.That(externalComponent.GetType(), Is.EqualTo(typeof(ExternalComponent1)));

            var mefComponent = container.GetExportedValue<IMefComponent>();
            Assert.That(mefComponent, Is.Not.Null);
            Assert.That(mefComponent.Component1.GetType(), Is.EqualTo(typeof(ExternalComponent1)));
            Assert.That(mefComponent.Component2.GetType(), Is.EqualTo(typeof(ExternalComponent1)));
        }

        [Test]
        public void ExportProviderResolvesServiceRegisteredByTypeAndRegistrationNameTest()
        {
            // Setup
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var provider = new FactoryExportProvider(FactoryMethod1);
            var container = new CompositionContainer(assemblyCatalog, provider);

            // Registration
            provider.Register(typeof(IExternalComponent), "external2");

            var externalComponent = container.GetExportedValue<IExternalComponent>("external2");
            Assert.That(externalComponent, Is.Not.Null);
            Assert.That(externalComponent.GetType(), Is.EqualTo(typeof(ExternalComponent2)));

            var mefComponent = container.GetExportedValue<IMefComponent>("component2");
            Assert.That(mefComponent, Is.Not.Null);
            Assert.That(mefComponent.Component1.GetType(), Is.EqualTo(typeof(ExternalComponent2)));
            Assert.That(mefComponent.Component2.GetType(), Is.EqualTo(typeof(ExternalComponent2)));
        }

        [Test]
        public void ExportProviderResolvesServiceRegisteredByTypeAndOrRegistrationNameTest()
        {
            // Setup
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var provider = new FactoryExportProvider(FactoryMethod1);
            var container = new CompositionContainer(assemblyCatalog, provider);

            // Registration
            provider.Register(typeof(IExternalComponent));
            provider.Register(typeof(IExternalComponent), "external2");
            
            var mefComponent = container.GetExportedValue<IMefComponent>("component3");
            Assert.That(mefComponent, Is.Not.Null);
            Assert.That(mefComponent.Component1.GetType(), Is.EqualTo(typeof(ExternalComponent1)));
            Assert.That(mefComponent.Component2.GetType(), Is.EqualTo(typeof(ExternalComponent2)));
        }

        [Test]
        public void ExportProviderProperlyResolvesServicesFromManyExportProvidersTest()
        {
            // Setup
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var provider1 = new FactoryExportProvider(FactoryMethod2_1);
            var provider2 = new FactoryExportProvider(FactoryMethod2_2);
            var container = new CompositionContainer(assemblyCatalog, provider1, provider2);

            // Registration
            provider1.Register(typeof(IExternalComponent));
            provider2.Register(typeof(IExternalComponent), "external2");

            var mefComponent = container.GetExportedValue<IMefComponent>("component3");
            Assert.That(mefComponent, Is.Not.Null);
            Assert.That(mefComponent.Component1.GetType(), Is.EqualTo(typeof(ExternalComponent1)));
            Assert.That(mefComponent.Component2.GetType(), Is.EqualTo(typeof(ExternalComponent2)));

            mefComponent = container.GetExportedValue<IMefComponent>("component4");
            Assert.That(mefComponent, Is.Not.Null);
            Assert.That(mefComponent.Component1.GetType(), Is.EqualTo(typeof(ExternalComponent1)));
            Assert.That(mefComponent.Component2.GetType(), Is.EqualTo(typeof(ExternalComponent2)));
        }

        private static object FactoryMethod1(Type type, string registrationName)
        {
            if (type == typeof(IExternalComponent) && registrationName == null)
                return new ExternalComponent1();

            if (type == typeof(IExternalComponent) && registrationName == "external2")
                return new ExternalComponent2();

            return null;
        }

        private static object FactoryMethod2_1(Type type, string registrationName)
        {
            if (type == typeof(IExternalComponent) && registrationName == null)
                return new ExternalComponent1();
            
            return null;
        }

        private static object FactoryMethod2_2(Type type, string registrationName)
        {
            if (type == typeof(IExternalComponent) && registrationName == "external2")
                return new ExternalComponent2();

            return null;
        }

        [Test]
        public void FactoryExportProviderResolvesServiceRegisteredUsingGivenFactoryMethodTest()
        {
            // Setup
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var provider = new FactoryExportProvider()
                .Register(typeof (IExternalComponent), () => new ExternalComponent1())
                .Register(typeof (ExternalComponent2), () => new ExternalComponent2());
            var container = new CompositionContainer(assemblyCatalog, provider);
            
            var externalComponent = container.GetExportedValue<IExternalComponent>();
            Assert.That(externalComponent, Is.Not.Null);
            Assert.That(externalComponent.GetType(), Is.EqualTo(typeof(ExternalComponent1)));

            var externalComponent2 = container.GetExportedValue<ExternalComponent2>();
            Assert.That(externalComponent2, Is.Not.Null);
            Assert.That(externalComponent2.GetType(), Is.EqualTo(typeof(ExternalComponent2)));

            var mefComponent = container.GetExportedValue<IMefComponent>();
            Assert.That(mefComponent, Is.Not.Null);
            Assert.That(mefComponent.Component1.GetType(), Is.EqualTo(typeof(ExternalComponent1)));
            Assert.That(mefComponent.Component2.GetType(), Is.EqualTo(typeof(ExternalComponent1)));
        }

        [Test]
        public void FactoryExportProviderExecutesTheFactoryEachTimeTheInstanceIsNeadedTest()
        {
            var count = 0;

            // Setup
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var provider = new FactoryExportProvider(typeof (ExternalComponent2), () =>
            {
                count++;
                return new ExternalComponent2();
            });
            var container = new CompositionContainer(assemblyCatalog, provider);

            var externalComponent1 = container.GetExportedValue<ExternalComponent2>();
            Assert.That(externalComponent1, Is.Not.Null);
            Assert.That(externalComponent1.GetType(), Is.EqualTo(typeof(ExternalComponent2)));

            var externalComponent2 = container.GetExportedValue<ExternalComponent2>();
            Assert.That(externalComponent2, Is.Not.Null);
            Assert.That(externalComponent2.GetType(), Is.EqualTo(typeof(ExternalComponent2)));

            Assert.That(count, Is.EqualTo(2));
        }
    }
}