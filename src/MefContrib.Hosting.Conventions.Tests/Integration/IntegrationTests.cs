using MefContrib.Tests;

namespace MefContrib.Hosting.Conventions.Tests.Integration
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Linq;
    using System.Reflection;

    using MefContrib.Hosting.Conventions.Configuration;
    using NUnit.Framework;

    [TestFixture]
    public class IntegrationTests
    {
        [Test]
        public void TargetName_should_TestExpectation()
        {
            var loader =
                new TypeLoader();
            loader.AddTypes(() => Assembly.GetExecutingAssembly().GetExportedTypes());

            var catalog =
                new AggregateCatalog();

            catalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetExecutingAssembly()));
            catalog.Catalogs.Add(new ConventionCatalog(new[] { new MyRegistry() }, loader));

            var instance =
                new Host();

            var batch =
                new CompositionBatch();
            batch.AddPart(instance);

            var container =
                new CompositionContainer(catalog);

            container.Compose(batch);

            instance.Widgets.Count().ShouldEqual(2);
        }

        [Test]
        public void Ctor_injection_test()
        {
            var loader =
                new TypeLoader();
            loader.AddTypes(() => Assembly.GetExecutingAssembly().GetExportedTypes());

            var catalog =
                new AggregateCatalog();

            catalog.Catalogs.Add(new ConventionCatalog(new[] { new CtorRegistry() }, loader));

            var instance =
                new ConventionPart<InjectedHost>();

            var batch =
                new CompositionBatch();
            batch.AddPart(instance);

            var container =
                new CompositionContainer(catalog);

            container.Compose(batch);

            instance.Imports[0].Widgets.Count().ShouldEqual(2);
        }
    }

    public class CtorRegistry : PartRegistry
    {
        public CtorRegistry()
        {
            Part()
                .ForTypesMatching(x => x.Name.Equals("InjectedHost"))
                .MakeShared()
                .ImportConstructor()
                .Exports(x => x.Export().Members(m => new[] { m }));

            Part()
                .ForTypesMatching(x => x.GetInterfaces().Contains(typeof(IWidget)))
                .ExportTypeAs<IWidget>()
                .MakeShared();
                
        }
    }

    public class InjectedHost
    {
        public InjectedHost(IEnumerable<IWidget> widgets)
        {
            this.Widgets = widgets;
        }

        public IEnumerable<IWidget> Widgets { get; set; }
    }

    public class MyRegistry : PartRegistry
    {
        public MyRegistry()
        {
            Part()
                .ForTypesMatching(x => x.GetInterfaces().Contains(typeof(IWidget)))
                .MakeShared()
                .Exports(x => x.Export().Members(m => new[] { m }).ContractType<IWidget>());
        }
    }

    public class Host
    {
        [ImportMany]
        public IEnumerable<IWidget> Widgets { get; set; }
    }

    public interface IWidget
    {
    }

    public class FooWidget : IWidget
    {
    }

    public class BarWidget : IWidget
    {
    }
}