#region Header

// -----------------------------------------------------------------------------
//  Copyright (c) Edenred (Incentives & Motivation) Ltd.  All rights reserved.
// -----------------------------------------------------------------------------

#endregion

namespace MefContrib.Integration.Autofac.Tests
{
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Reflection;
    using global::Autofac;
    using NUnit.Framework;

    [TestFixture]
    public class BidirectionalIntegrationTests
    {
        public class MefSingletonComponent
        {
            #region Fields

            public static int Counter;

            #endregion

            #region Constructors

            public MefSingletonComponent()
            {
                Counter++;
            }

            #endregion
        }

        [Test]
        public void AutofacCanResolveAutofacComponentThatHasAutofacAndMefDependenciesTest()
        {
            // Setup
            var builder = new ContainerBuilder();
            builder.EnableCompositionIntegration();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Register catalog and types
            builder.RegisterCatalog(assemblyCatalog);
            builder.RegisterType<AutofacOnlyComponent1>().As<IAutofacOnlyComponent>();
            builder.RegisterType<AutofacMixedComponent>();

            // Test
            var container = builder.Build();
            var autofacMixedComponent = container.Resolve<AutofacMixedComponent>();
            Assert.That(autofacMixedComponent, Is.Not.Null);
            Assert.That(autofacMixedComponent.GetType(), Is.EqualTo(typeof (AutofacMixedComponent)));
            Assert.That(autofacMixedComponent.MefComponent.GetType(), Is.EqualTo(typeof (MefComponent1)));
            Assert.That(autofacMixedComponent.AutofacComponent.GetType(), Is.EqualTo(typeof (AutofacOnlyComponent1)));
        }

        [Test]
        public void AutofacCanResolveMefComponentRegisteredUsingAddExportedValueTest()
        {
            MefSingletonComponent.Counter = 0;

            // Setup
            var builder = new ContainerBuilder();
            builder.EnableCompositionIntegration();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Register catalog and types
            builder.RegisterCatalog(assemblyCatalog);
            var autofacContainer = builder.Build();
            var compositionContainer = autofacContainer.Resolve<CompositionContainer>();
            var batch = new CompositionBatch();
            var singletonComponent = new MefSingletonComponent();

            batch.AddExportedValue(singletonComponent);
            compositionContainer.Compose(batch);

            var singletonComponent1 = compositionContainer.GetExport<MefSingletonComponent>().Value;
            Assert.That(MefSingletonComponent.Counter, Is.EqualTo(1));
            Assert.That(singletonComponent1, Is.SameAs(singletonComponent));

            var singletonComponent2 = autofacContainer.Resolve<MefSingletonComponent>();
            Assert.That(MefSingletonComponent.Counter, Is.EqualTo(1));
            Assert.That(singletonComponent2, Is.SameAs(singletonComponent));
        }

        [Test]
        public void AutofacCanResolveMefComponentThatHasAutofacDependenciesTest()
        {
            // Setup
            var builder = new ContainerBuilder();
            builder.EnableCompositionIntegration();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Register catalog and types
            builder.RegisterCatalog(assemblyCatalog);
            builder.RegisterType<AutofacOnlyComponent1>().As<IAutofacOnlyComponent>();

            // Test
            var container = builder.Build();
            var mefComponent = container.Resolve<IMefComponentWithAutofacDependencies>();
            Assert.That(mefComponent, Is.Not.Null);
            Assert.That(mefComponent.MefOnlyComponent.GetType(), Is.EqualTo(typeof (MefComponent1)));
            Assert.That(mefComponent.AutofacOnlyComponent.GetType(), Is.EqualTo(typeof (AutofacOnlyComponent1)));
        }

        [Test]
        public void AutofacCanResolveMefComponentThatHasAutofacDependenciesThatHaveMefDependenciesTest()
        {
            // Setup
            var builder = new ContainerBuilder();
            builder.EnableCompositionIntegration();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Register catalog and types
            builder.RegisterCatalog(assemblyCatalog);
            builder.RegisterType<AutofacOnlyComponent1>().As<IAutofacOnlyComponent>();
            builder.RegisterType<AutofacComponent1>().As<IAutofacComponent>();

            // Test
            var container = builder.Build();
            var mefComponent = container.ResolveNamed<IMefComponentWithAutofacDependencies>("component2");
            Assert.That(mefComponent, Is.Not.Null);
            Assert.That(mefComponent.GetType(), Is.EqualTo(typeof (MefComponentWithAutofacDependencies2)));
            Assert.That(mefComponent.MefOnlyComponent.GetType(), Is.EqualTo(typeof (MefComponent1)));
            Assert.That(mefComponent.AutofacOnlyComponent.GetType(), Is.EqualTo(typeof (AutofacOnlyComponent1)));

            var mefComponentWithAutofacDependencies2 = (MefComponentWithAutofacDependencies2) mefComponent;
            Assert.That(mefComponentWithAutofacDependencies2.MixedAutofacMefComponent.GetType(),
                        Is.EqualTo(typeof (AutofacComponent1)));
            Assert.That(mefComponentWithAutofacDependencies2.MixedAutofacMefComponent.MefComponent.GetType(),
                        Is.EqualTo(typeof (MefComponent1)));
        }

        [Test]
        public void AutofacCircularDependencyIsDetectedTest()
        {
            // Setup
            var builder = new ContainerBuilder();
            builder.EnableCompositionIntegration();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Register catalog and types
            builder.RegisterCatalog(assemblyCatalog);
            builder.RegisterType<AutofacOnlyComponent1>();

            // Test
            var container = builder.Build();
            var autofacOnlyComponent1 = container.Resolve<AutofacOnlyComponent1>();
            Assert.That(autofacOnlyComponent1, Is.Not.Null);
        }

        [Test]
        public void AutofacContainerCanBeResolvedByMefTest()
        {
            // Setup
            var builder = new ContainerBuilder();
            builder.EnableCompositionIntegration();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Register catalog and types
            builder.RegisterCatalog(assemblyCatalog);

            // Test
            var container = builder.Build();
            var compositionContainer1 = container.Resolve<CompositionContainer>();
            var compositionContainer2 = container.Resolve<CompositionContainer>();
            Assert.That(compositionContainer1, Is.Not.Null);
            Assert.That(compositionContainer2, Is.Not.Null);
            Assert.That(compositionContainer1, Is.SameAs(compositionContainer2));

            var autofacContainerFromMef1 = compositionContainer1.GetExportedValue<ILifetimeScope>();
            var autofacContainerFromMef2 = compositionContainer1.GetExportedValue<ILifetimeScope>();

            Assert.That(autofacContainerFromMef1, Is.Not.Null);
            Assert.That(autofacContainerFromMef2, Is.Not.Null);
            Assert.AreSame(autofacContainerFromMef1, autofacContainerFromMef2);
        }

        [Test]
        public void AutofacResolvesAutofacComponentRegisteredWithoutInterfaceTest()
        {
            // Setup
            var builder = new ContainerBuilder();
            builder.EnableCompositionIntegration();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Register catalog and types
            builder.RegisterCatalog(assemblyCatalog);

            // Registration
            builder.Register((c, p) => new AutofacComponent2(c.ResolveNamed<IMefComponent>("component2")));
            builder.Register((c, p) => new AutofacComponent3(c.ResolveNamed<IMefComponent>("component2")));
            var container = builder.Build();
            var component2 = container.Resolve<AutofacComponent2>();
            Assert.That(component2, Is.Not.Null);
            Assert.That(component2.ImportedMefComponent, Is.Not.Null);
            Assert.That(component2.ImportedMefComponent.GetType(), Is.EqualTo(typeof (MefComponent2)));
            Assert.That(component2.MefComponent.GetType(), Is.EqualTo(typeof (MefComponent2)));
        }

        [Test]
        public void MefCanResolveMefComponentThatHasAutofacAndMefDependenciesTest()
        {
            // Setup
            var builder = new ContainerBuilder();
            builder.EnableCompositionIntegration();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Register catalog and types
            builder.RegisterCatalog(assemblyCatalog);
            builder.RegisterType<AutofacOnlyComponent1>().As<IAutofacOnlyComponent>();

            // Test
            var autofacContainer = builder.Build();
            var compositionContainer = autofacContainer.Resolve<CompositionContainer>();
            var mefMixedComponent = compositionContainer.GetExportedValue<MefMixedComponent>();
            Assert.That(mefMixedComponent, Is.Not.Null);
            Assert.That(mefMixedComponent.GetType(), Is.EqualTo(typeof (MefMixedComponent)));
            Assert.That(mefMixedComponent.MefComponent.GetType(), Is.EqualTo(typeof (MefComponent1)));
            Assert.That(mefMixedComponent.AutofacComponent.GetType(), Is.EqualTo(typeof (AutofacOnlyComponent1)));
        }

        [Test]
        public void MefResolvesServiceRegisteredInAutofacByTypeTest()
        {
            // Setup
            var builder = new ContainerBuilder();
            builder.EnableCompositionIntegration();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Register catalog and types
            builder.RegisterCatalog(assemblyCatalog);

            // Registration
            builder.RegisterType<AutofacOnlyComponent1>().As<IAutofacOnlyComponent>().InstancePerLifetimeScope();

            var autofacContainer = builder.Build();
            var compositionContainer = autofacContainer.Resolve<CompositionContainer>();
            var fromMef = compositionContainer.GetExportedValue<IAutofacOnlyComponent>();
            var fromAutofac = autofacContainer.Resolve<IAutofacOnlyComponent>();
            Assert.That(fromMef, Is.Not.Null);
            Assert.That(fromMef.GetType(), Is.EqualTo(typeof (AutofacOnlyComponent1)));
            Assert.That(fromAutofac, Is.Not.Null);
            Assert.That(fromAutofac.GetType(), Is.EqualTo(typeof (AutofacOnlyComponent1)));
            Assert.That(fromMef, Is.EqualTo(fromAutofac));
        }
    }
}