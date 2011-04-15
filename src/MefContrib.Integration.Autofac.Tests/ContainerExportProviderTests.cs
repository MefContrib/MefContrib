namespace MefContrib.Integration.Autofac.Tests
{
    using System;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Linq;
    using System.Reflection;
    using global::Autofac;
    using MefContrib.Containers;
    using NUnit.Framework;

    [TestFixture]
    public class ContainerExportProviderTests
    {
        [Test]
        public void ExportProviderResolvesServiceRegisteredByTypeTest()
        {
            // Setup
            var builder = new ContainerBuilder();
            builder.RegisterType<AutofacOnlyComponent1>().As<IAutofacOnlyComponent>();
            var autofacContainer = builder.Build();
            var adapter = new AutofacContainerAdapter(autofacContainer);
            var provider = new ContainerExportProvider(adapter);

            var component = provider.GetExportedValue<IAutofacOnlyComponent>();
            Assert.That(component, Is.Not.Null);
            Assert.That(component.GetType(), Is.EqualTo(typeof(AutofacOnlyComponent1)));
        }

        [Test]
        public void ExportProviderResolvesServicesRegisteredByTypeTest()
        {
            // Setup
            var builder = new ContainerBuilder();
            builder.RegisterType<AutofacOnlyComponent1>().As<IAutofacOnlyComponent>();
            builder.RegisterType<AutofacOnlyComponent2>().Named<IAutofacOnlyComponent>("b");
            var autofacContainer = builder.Build();
            var adapter = new AutofacContainerAdapter(autofacContainer);
            var provider = new ContainerExportProvider(adapter);

            var components = provider.GetExports<IAutofacOnlyComponent>();
            Assert.That(components, Is.Not.Null);
            Assert.That(components.Count(), Is.EqualTo(2));

            Assert.That(components.Select(t => t.Value).OfType<AutofacOnlyComponent1>().Count(), Is.EqualTo(1));
            Assert.That(components.Select(t => t.Value).OfType<AutofacOnlyComponent2>().Count(), Is.EqualTo(1));
        }

        [Test]
        public void ExportProviderResolvesServiceRegisteredByTypeAndRegistrationNameTest()
        {
            // Setup
            var builder = new ContainerBuilder();
            builder.RegisterType<AutofacOnlyComponent1>().As<IAutofacOnlyComponent>();
            builder.RegisterType<AutofacOnlyComponent2>().Named<IAutofacOnlyComponent>("autofacComponent2");
            var autofacContainer = builder.Build();
            var adapter = new AutofacContainerAdapter(autofacContainer);
            var provider = new ContainerExportProvider(adapter);

            var component = provider.GetExportedValue<IAutofacOnlyComponent>("autofacComponent2");
            Assert.That(component, Is.Not.Null);
            Assert.That(component.GetType(), Is.EqualTo(typeof(AutofacOnlyComponent2)));
        }

        [Test]
        public void MefCanResolveLazyTypeRegisteredInAutofacTest()
        {
            // Setup
            var builder = new ContainerBuilder();
            builder.RegisterType<AutofacOnlyComponent1>().As<IAutofacOnlyComponent>();
            var autofacContainer = builder.Build();
            var adapter = new AutofacContainerAdapter(autofacContainer);
            var provider = new ContainerExportProvider(adapter);
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var container = new CompositionContainer(assemblyCatalog, provider);

            AutofacOnlyComponent1.InstanceCount = 0;

            var lazyAutofacComponent = container.GetExport<IAutofacOnlyComponent>();
            Assert.That(lazyAutofacComponent, Is.Not.Null);
            Assert.That(AutofacOnlyComponent1.InstanceCount, Is.EqualTo(0));

            Assert.That(lazyAutofacComponent.Value, Is.Not.Null);
            Assert.That(lazyAutofacComponent.Value.GetType(), Is.EqualTo(typeof(AutofacOnlyComponent1)));
            Assert.That(AutofacOnlyComponent1.InstanceCount, Is.EqualTo(1));
        }

        [Test]
        public void MefCanResolveLazyTypesRegisteredInAutofacTest()
        {
            // Setup
            var builder = new ContainerBuilder();
            builder.RegisterType<AutofacOnlyComponent1>().As<IAutofacOnlyComponent>();
            builder.RegisterType<AutofacOnlyComponent2>().Named<IAutofacOnlyComponent>("a");
            var autofacContainer = builder.Build();
            var adapter = new AutofacContainerAdapter(autofacContainer);
            var provider = new ContainerExportProvider(adapter);
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var container = new CompositionContainer(assemblyCatalog, provider);

            AutofacOnlyComponent1.InstanceCount = 0;

            var lazyAutofacComponent = container.GetExports<IAutofacOnlyComponent>().ToList();
            Assert.That(lazyAutofacComponent, Is.Not.Null);
            Assert.That(AutofacOnlyComponent1.InstanceCount, Is.EqualTo(0));

            Assert.That(lazyAutofacComponent, Is.Not.Null);
            Assert.That(lazyAutofacComponent[0].Value, Is.Not.Null);
            Assert.That(lazyAutofacComponent[1].Value, Is.Not.Null);
            Assert.That(AutofacOnlyComponent1.InstanceCount, Is.EqualTo(1));
        }

        [Test]
        public void MefCanResolveTypesRegisteredInAutofacAfterTrackingExtensionIsAddedTest()
        {
            // Setup
            var builder = new ContainerBuilder();
            builder.RegisterType<AutofacOnlyComponent2>().Named<IAutofacOnlyComponent>("autofacComponent2");
            var autofacContainer = builder.Build();

            // Further setup
            var adapter = new AutofacContainerAdapter(autofacContainer);
            var provider = new ContainerExportProvider(adapter);
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var container = new CompositionContainer(assemblyCatalog, provider);

            var component = container.GetExportedValue<IAutofacOnlyComponent>("autofacComponent2");
            Assert.That(component, Is.Not.Null);
            Assert.That(component.GetType(), Is.EqualTo(typeof(AutofacOnlyComponent2)));
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
        public void CannotPassNullAutofacInstanceToTheAutofacContainerAdapterConstructorTest()
        {
            Assert.That(delegate
            {
                new AutofacContainerAdapter(null);
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
            var builder1 = new ContainerBuilder();
            var a = new A();
            builder1.RegisterInstance(a);
            var autofacContainer1 = builder1.Build();
            var exportProvider1 = new ContainerExportProvider(new AutofacContainerAdapter(autofacContainer1));

            var builder2 = new ContainerBuilder();
            var b = new B();
            builder2.RegisterInstance(b);
            var autofacContainer2 = builder2.Build();
            var exportProvider2 = new ContainerExportProvider(new AutofacContainerAdapter(autofacContainer2));

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