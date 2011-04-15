namespace MefContrib.Integration.Autofac.Tests
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using global::Autofac;
    using global::Autofac.Core;
    using NUnit.Framework;

    [TestFixture]
    public class CompositionIntegrationTests
    {

        public static class CompositionContainerExporter
        {
            static CompositionContainerExporter()
            {
                Container = new CompositionContainer();
            }

            [Export]
            public static CompositionContainer Container { get; set; }
        }
        [Test]
        public void AutofacCanResolveMefComponentRegisteredByTypeTest()
        {
            // Setup
            var builder = new ContainerBuilder();
            builder.EnableCompositionIntegration();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Add composition support for autofac
            builder.EnableCompositionIntegration();
            builder.RegisterCatalog(assemblyCatalog);

            builder.RegisterType<AutofacComponent1>().As<IAutofacComponent>();

            var autofacContainer = builder.Build();
            var mefComponent = autofacContainer.Resolve<IMefComponent>();
            Assert.That(mefComponent, Is.Not.Null);
            Assert.That(mefComponent.GetType(), Is.EqualTo(typeof(MefComponent1)));

            var autofacComponent = autofacContainer.Resolve<IAutofacComponent>();
            Assert.That(autofacComponent, Is.Not.Null);
            Assert.That(autofacComponent.MefComponent.GetType(), Is.EqualTo(typeof(MefComponent1)));
        }

        [Test]
        public void AutofacCanResolveMefComponentRegisteredByTypeAndRegistrationNameTest()
        {
            // Setup
            var builder = new ContainerBuilder();
            builder.EnableCompositionIntegration();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Add composition support for autofac
            builder.EnableCompositionIntegration();
            builder.RegisterCatalog(assemblyCatalog);
            builder.Register((c, p) => new AutofacComponent2(c.ResolveNamed<IMefComponent>("component2"))).As<IAutofacComponent>();

            var autofacContainer = builder.Build();
            var mefComponent = autofacContainer.ResolveNamed<IMefComponent>("component2");
            Assert.That(mefComponent, Is.Not.Null);
            Assert.That(mefComponent.GetType(), Is.EqualTo(typeof(MefComponent2)));

            var autofacComponent = autofacContainer.Resolve<IAutofacComponent>();
            Assert.That(autofacComponent, Is.Not.Null);
            Assert.That(autofacComponent.MefComponent.GetType(), Is.EqualTo(typeof(MefComponent2)));
        }

        [Test]
        public void AutofacCanResolveMefComponentRegisteredByTypeUsingConstructorInjectionTest()
        {
            // Setup
            var builder = new ContainerBuilder();
            builder.EnableCompositionIntegration();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Add composition support for autofac
            builder.EnableCompositionIntegration();
            builder.RegisterCatalog(assemblyCatalog);
            builder.RegisterType<AutofacComponent1>().As<IAutofacComponent>();

            var autofacContainer = builder.Build();
            var autofacComponent = autofacContainer.Resolve<IAutofacComponent>();
            Assert.That(autofacComponent, Is.Not.Null);
            Assert.That(autofacComponent.MefComponent.GetType(), Is.EqualTo(typeof(MefComponent1)));
        }

        [Test]
        public void AutofacCanResolveMefComponentRegisteredByTypeAndRegistrationNameUsingConstructorInjectionTest()
        {
            // Setup
            var builder = new ContainerBuilder();
            builder.EnableCompositionIntegration();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Add composition support for autofac
            builder.EnableCompositionIntegration();
            builder.RegisterCatalog(assemblyCatalog);
            builder.Register((c, p) => new AutofacComponent2(c.ResolveNamed<IMefComponent>("component2"))).As<IAutofacComponent>();

            var autofacContainer = builder.Build();
            var autofacComponent = autofacContainer.Resolve<IAutofacComponent>();
            Assert.That(autofacComponent, Is.Not.Null);
            Assert.That(autofacComponent.MefComponent.GetType(), Is.EqualTo(typeof(MefComponent2)));
        }

        [Test]
        public void AutofacSatisfiesMefImportsByTypeOnAutofacComponentsTest()
        {
            // Setup
            var builder = new ContainerBuilder();
            builder.EnableCompositionIntegration();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Add composition support for autofac
            builder.EnableCompositionIntegration();
            builder.RegisterCatalog(assemblyCatalog);
            builder.RegisterType<AutofacComponent1>().As<IAutofacComponent>();

            var autofacContainer = builder.Build();
            var autofacComponent = autofacContainer.Resolve<IAutofacComponent>();
            Assert.That(autofacComponent, Is.Not.Null);
            Assert.That(autofacComponent.ImportedMefComponent.GetType(), Is.EqualTo(typeof(MefComponent1)));
        }

        [Test]
        public void AutofacLazySatisfiesMefImportsByTypeOnAutofacComponentsTest()
        {
            // Setup
            var builder = new ContainerBuilder();
            builder.EnableCompositionIntegration();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Add composition support for autofac
            builder.EnableCompositionIntegration();
            builder.RegisterCatalog(assemblyCatalog);
            builder.RegisterType<AutofacComponent1>().As<IAutofacComponent>();

            builder.RegisterType<AutofacComponent11>().As<IAutofacComponent>();

            var autofacContainer = builder.Build();
            var autofacComponent = autofacContainer.Resolve<IAutofacComponent>();
            Assert.That(autofacComponent, Is.Not.Null);
            Assert.That(autofacComponent.GetType(), Is.EqualTo(typeof(AutofacComponent11)));
            Assert.That(autofacComponent.ImportedMefComponent.GetType(), Is.EqualTo(typeof(MefComponent1)));
            Assert.That(autofacComponent.MefComponent.GetType(), Is.EqualTo(typeof(MefComponent1)));

            var autofacComponent11 = (AutofacComponent11)autofacComponent;
            var mefComponent = autofacComponent11.MefComponentFactory();
            Assert.That(mefComponent, Is.SameAs(autofacComponent.MefComponent));
        }

        [Test]
        public void AutofacSatisfiesMefImportsByTypeAndRegistrationNameOnAutofacComponentsTest()
        {
            // Setup
            var builder = new ContainerBuilder();
            builder.EnableCompositionIntegration();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Add composition support for autofac
            builder.EnableCompositionIntegration();
            builder.RegisterCatalog(assemblyCatalog);
            builder.Register((c, p) => new AutofacComponent2(c.ResolveNamed<IMefComponent>("component2"))).As<IAutofacComponent>();

            var autofacContainer = builder.Build();
            var autofacComponent = autofacContainer.Resolve<IAutofacComponent>();
            Assert.That(autofacComponent, Is.Not.Null);
            Assert.That(autofacComponent.ImportedMefComponent.GetType(), Is.EqualTo(typeof(MefComponent2)));
        }

        [Test]
        public void AutofacDoesNotSatisfyMefImportsOnAutofacComponentsWhenMarkedWithPartNotComposableAttributeTest()
        {
            // Setup
            var builder = new ContainerBuilder();
            builder.EnableCompositionIntegration();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Add composition support for autofac
            builder.EnableCompositionIntegration();
            builder.RegisterCatalog(assemblyCatalog);
            builder.Register((c, p) => new AutofacComponent3(c.ResolveNamed<IMefComponent>("component2"))).As<IAutofacComponent>();

            var autofacContainer = builder.Build();
            var autofacComponent = autofacContainer.Resolve<IAutofacComponent>();
            Assert.That(autofacComponent, Is.Not.Null);
            Assert.That(autofacComponent.ImportedMefComponent, Is.Null);
        }

        [Test]
        public void AutofacCanResolveCompositionContainerTest()
        {
            // Setup
            var builder = new ContainerBuilder();
            builder.EnableCompositionIntegration();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Add composition support for autofac
            builder.EnableCompositionIntegration();
            builder.RegisterCatalog(assemblyCatalog);

            var autofacContainer = builder.Build();
            var compositionContainer = autofacContainer.Resolve<CompositionContainer>();
            Assert.That(compositionContainer, Is.Not.Null);
        }

        [Test]
        public void AutofacCanResolveMultipleMefInstancesTest()
        {
            // Setup
            var builder = new ContainerBuilder();
            builder.EnableCompositionIntegration();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Add composition support for autofac
            builder.EnableCompositionIntegration();
            builder.RegisterCatalog(assemblyCatalog);

            var autofacContainer = builder.Build();
            Assert.That(delegate
            {
                var defaultInstance = autofacContainer.Resolve<IMultipleMefComponent>();
                Debug.WriteLine("Default Instance -> {0}", defaultInstance);
                var all = autofacContainer.Resolve<IEnumerable<IMultipleMefComponent>>().ToArray();
                Debug.WriteLine("All instances -> {0}, {1}", all);
            }, Throws.Nothing);
        }

        [Test]
        public void DisposingAutofacDisposesCompositionContainerTest()
        {
            // Setup
            var builder = new ContainerBuilder();
            builder.EnableCompositionIntegration();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Add composition support for autofac
            builder.EnableCompositionIntegration();
            builder.RegisterCatalog(assemblyCatalog);

            var autofacContainer = builder.Build();
            var compositionContainer = autofacContainer.Resolve<CompositionContainer>();
            autofacContainer.Dispose();

            Assert.That(delegate
            {
                compositionContainer.GetExport<IMefComponent>();
            }, Throws.TypeOf<ObjectDisposedException>());
        }
    }
}