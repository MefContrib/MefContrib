namespace MefContrib.Integration.Autofac.Tests
{
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Reflection;
    using global::Autofac;
    using NUnit.Framework;
    using IAutofacContainer = global::Autofac.ILifetimeScope;

    [TestFixture]
    public class ContainerExtensionsTests
    {
        public interface IMefExposedComponent
        {
        }

        [Export(typeof(IMefExposedComponent))]
        public class MefComponent : IMefExposedComponent
        {
        }

        public interface IAutofacComponent
        {
        }

        public class AutofacComponent : IAutofacComponent
        {
        }

        public interface IMefDependingOnAutofac
        {
            IAutofacComponent AutofacComponent { get; }
        }

        public interface IMefDependingOnAutofac2
        {
            IAutofacComponent AutofacComponent { get; }
        }

        [Export(typeof(IMefDependingOnAutofac))]
        public class MefComponentDependingOnAutofac : IMefDependingOnAutofac
        {
            private readonly IAutofacComponent m_AutofacComponent;

            [ImportingConstructor]
            public MefComponentDependingOnAutofac(IAutofacComponent autofacComponent)
            {
                m_AutofacComponent = autofacComponent;
            }

            public IAutofacComponent AutofacComponent { get { return m_AutofacComponent; } }
        }

        [Export(typeof(IMefDependingOnAutofac2))]
        public class MefComponentDependingOnAutofacByProperty : IMefDependingOnAutofac2
        {
            [Import]
            public IAutofacComponent AutofacComponent { get; set; }
        }

        private static IContainer ConfigureAutofacThenMef()
        {
            var builder  = new ContainerBuilder();

            var catalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            builder.RegisterType<AutofacComponent>().As<IAutofacComponent>();
            builder.RegisterType<CountableAutofacComponent>().As<ICountableAutofacComponent>();
            var container = builder.Build();
            container.RegisterCatalog(catalog);

            return container;
        }

        private static IContainer ConfigureMefThenAutofac()
        {
            var builder = new ContainerBuilder();
            builder.EnableCompositionIntegration();
            var catalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            builder.RegisterCatalog(catalog);
            builder.RegisterType<AutofacComponent>().As<IAutofacComponent>();
            builder.RegisterType<CountableAutofacComponent>().As<ICountableAutofacComponent>();

            return builder.Build();
        }

        [Test]
        public void ResolveAutofacFromAutofacTest()
        {
            var container = ConfigureAutofacThenMef();

            var component = container.Resolve<IAutofacComponent>();
            Assert.IsNotNull(component);
        }

        [Test]
        public void ResolveAutofacFromAutofac2Test()
        {
            var container = ConfigureMefThenAutofac();

            var component = container.Resolve<IAutofacComponent>();
            Assert.IsNotNull(component);
        }

        [Test]
        public void ResolveMefFromAutofacTest()
        {
            var container = ConfigureAutofacThenMef();

            var component = container.Resolve<IMefExposedComponent>();
            Assert.IsNotNull(component);
        }
        [Test]
        public void ResolveMefFromAutofac2Test()
        {
            var container = ConfigureMefThenAutofac();

            var component = container.Resolve<IMefExposedComponent>();
            Assert.IsNotNull(component);
        }

        [Test]
        public void ResolveAutofacFromMefByCtorTest()
        {
            var container = ConfigureAutofacThenMef();

            var autofacComponent = container.Resolve<IAutofacComponent>();

            var component = container.Resolve<IMefDependingOnAutofac>();
            Assert.IsNotNull(component);
            Assert.IsNotNull(component.AutofacComponent);
            Assert.AreNotEqual(autofacComponent, component.AutofacComponent);
        }

        [Test]
        public void ResolveAutofacFromMefByCtor2Test()
        {
            var container = ConfigureMefThenAutofac();

            var autofacComponent = container.Resolve<IAutofacComponent>();

            var component = container.Resolve<IMefDependingOnAutofac>();
            Assert.IsNotNull(component);
            Assert.IsNotNull(component.AutofacComponent);
            Assert.AreNotEqual(autofacComponent, component.AutofacComponent);
        }

        [Test]
        public void ResolveAutofacFromMefByPropTest()
        {
            var container = ConfigureAutofacThenMef();

            var autofacComponent = container.Resolve<IAutofacComponent>();

            var component = container.Resolve<IMefDependingOnAutofac2>();
            Assert.IsNotNull(component);
            Assert.IsNotNull(component.AutofacComponent);
            Assert.AreNotEqual(autofacComponent, component.AutofacComponent);
        }

        [Test]
        public void ResolveAutofacFromMefByProp2Test()
        {
            var container = ConfigureMefThenAutofac();

            var autofacComponent = container.Resolve<IAutofacComponent>();

            var component = container.Resolve<IMefDependingOnAutofac2>();
            Assert.IsNotNull(component);
            Assert.IsNotNull(component.AutofacComponent);
            Assert.AreNotEqual(autofacComponent, component.AutofacComponent);
        }

        [Test]
        public void ResolveAutofacFromMefByCtorWithScopeTest()
        {
            IAutofacContainer container = ConfigureAutofacThenMef();

            var autofacComponent = container.Resolve<IAutofacComponent>();
            container = container.BeginLifetimeScope();
            var builder = new ContainerBuilder();
            builder.RegisterInstance(autofacComponent);
            builder.Update(container.ComponentRegistry);

            var component = container.Resolve<IMefDependingOnAutofac>();
            Assert.IsNotNull(component);
            Assert.IsNotNull(component.AutofacComponent);
            Assert.AreEqual(autofacComponent, component.AutofacComponent);
        }

        [Test]
        public void ResolveAutofacFromMefByCtorWithScope2Test()
        {
            IAutofacContainer container = ConfigureMefThenAutofac();

            var autofacComponent = container.Resolve<IAutofacComponent>();
            container = container.BeginLifetimeScope();
            container.EnableCompositionIntegration();
            var builder = new ContainerBuilder();
            builder.RegisterInstance(autofacComponent);
            builder.Update(container.ComponentRegistry);

            var component = container.Resolve<IMefDependingOnAutofac>();
            Assert.IsNotNull(component);
            Assert.IsNotNull(component.AutofacComponent);
            Assert.AreEqual(autofacComponent, component.AutofacComponent);
        }

        [Test]
        public void ResolveAutofacFromMefByPropWithScopeTest()
        {
            IAutofacContainer container = ConfigureAutofacThenMef();

            var autofacComponent = container.Resolve<IAutofacComponent>();
            container = container.BeginLifetimeScope();
            container.EnableCompositionIntegration();
            var builder = new ContainerBuilder();
            builder.RegisterInstance(autofacComponent);
            builder.Update(container.ComponentRegistry);

            var component = container.Resolve<IMefDependingOnAutofac2>();
            Assert.IsNotNull(component);
            Assert.IsNotNull(component.AutofacComponent);
            Assert.AreEqual(autofacComponent, component.AutofacComponent);
        }

        [Test]
        public void ResolveAutofacFromMefByPropWithScope2Test()
        {
            IAutofacContainer container = ConfigureMefThenAutofac();

            var autofacComponent = container.Resolve<IAutofacComponent>();
            container = container.BeginLifetimeScope();
            container.EnableCompositionIntegration();
            var builder = new ContainerBuilder();
            builder.RegisterInstance(autofacComponent);
            builder.Update(container.ComponentRegistry);

            var component = container.Resolve<IMefDependingOnAutofac2>();
            Assert.IsNotNull(component);
            Assert.IsNotNull(component.AutofacComponent);
            Assert.AreEqual(autofacComponent, component.AutofacComponent);
        }


        public interface ICountableAutofacComponent
        {
            int InstanceCount { get; }
        }

        public interface ICountableMefComponent
        {
            int InstanceCount { get; }
        }

        public class CountableAutofacComponent : ICountableAutofacComponent
        {
            private static int m_InstanceCount;

            public CountableAutofacComponent()
            {
                ++m_InstanceCount;
            }

            public static void ResetInstanceCount()
            {
                m_InstanceCount = 0;
            }

            public int InstanceCount { get { return m_InstanceCount; } }
        }

        [Export(typeof(ICountableMefComponent))]
        public class CountableMefComponent : ICountableMefComponent
        {
            private static int m_InstanceCount;

            public CountableMefComponent()
            {
                ++m_InstanceCount;
            }

            public static void ResetInstanceCount()
            {
                m_InstanceCount = 0;
            }

            public int InstanceCount { get { return m_InstanceCount; } }
        }

        [Test]
        public void AutofacInstanceCountTest()
        {
            CountableAutofacComponent.ResetInstanceCount();
            IAutofacContainer container = ConfigureAutofacThenMef();
            var countable = container.Resolve<ICountableAutofacComponent>();
            Assert.AreEqual(1, countable.InstanceCount);
        }

        [Test]
        public void AutofacInstanceCount2Test()
        {
            CountableAutofacComponent.ResetInstanceCount();
            IAutofacContainer container = ConfigureMefThenAutofac();
            var countable = container.Resolve<ICountableAutofacComponent>();
            Assert.AreEqual(1, countable.InstanceCount);
        }

        [Test]
        public void MefInstanceCountTest()
        {
            CountableMefComponent.ResetInstanceCount();
            IAutofacContainer container = ConfigureAutofacThenMef();
            var countable = container.Resolve<ICountableMefComponent>();
            Assert.AreEqual(1, countable.InstanceCount);
        }

        [Test]
        public void MefInstanceCount2Test()
        {
            CountableMefComponent.ResetInstanceCount();
            IAutofacContainer container = ConfigureMefThenAutofac();
            var countable = container.Resolve<ICountableMefComponent>();
            Assert.AreEqual(1, countable.InstanceCount);
        }

        public interface IDependOnCountableAutofac
        {
            ICountableAutofacComponent Component { get; set; }
        }

        [Export(typeof(IDependOnCountableAutofac))]
        public class DependOnCountableAutofac : IDependOnCountableAutofac
        {
            [Import]
            public ICountableAutofacComponent Component { get; set; }
        }

        [Test]
        public void AutofacInstanceCountDepScopedTest()
        {
            CountableAutofacComponent.ResetInstanceCount();
            IAutofacContainer container = ConfigureAutofacThenMef();
            var childContainer = container.BeginLifetimeScope();
            var countableDep = childContainer.Resolve<IDependOnCountableAutofac>();
            Assert.AreEqual(1, countableDep.Component.InstanceCount);
        }
    }
}