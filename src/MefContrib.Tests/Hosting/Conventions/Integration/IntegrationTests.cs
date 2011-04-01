namespace MefContrib.Hosting.Conventions.Tests.Integration
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Linq;
    using System.Reflection;
    using MefContrib.Hosting.Conventions.Configuration;
    using MefContrib.Tests;
    using NUnit.Framework;

    [TestFixture, Category("Integration")]
    public class IntegrationTests
    {
        [Test]
        public void ConventionCatalog_should_support_constructor_injection()
        {
            var catalog =
                new ConventionCatalog(new CtorRegistry());

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

        [Test]
        public void ConventionCatalog_should_support_type_exports()
        {
            var registry = new PartRegistry();
            registry.TypeScanner = new AssemblyTypeScanner(Assembly.GetExecutingAssembly());

            registry
                .Part()
                .ForType<SampleExport>()
                .Export();

            var catalog =
               new ConventionCatalog(registry);

            var instance =
                new ConventionPart<SampleExport>();

            var batch =
                new CompositionBatch();
            batch.AddPart(instance);

            var container =
                new CompositionContainer(catalog);

            container.Compose(batch);

            instance.Imports.Count().ShouldEqual(1);
        }

        [Test]
        public void ConventionCatalog_should_support_property_exports()
        {
            var registry = new PartRegistry();
            registry.TypeScanner = new AssemblyTypeScanner(Assembly.GetExecutingAssembly());

            registry
                .Part()
                .ForType<SampleExport>()
                .ExportProperty("TextValue", "V1");
            
            var catalog =
               new ConventionCatalog(registry);
            
            var container =
                new CompositionContainer(catalog);

            var exportedValue = container.GetExportedValue<string>("V1");
            Assert.That(exportedValue, Is.EqualTo("this is some text"));
        }

        [Test]
        public void ConventionCatalog_should_support_field_exports()
        {
            var registry = new PartRegistry();
            registry.TypeScanner = new AssemblyTypeScanner(Assembly.GetExecutingAssembly());

            registry
                .Part()
                .ForType<SampleExport>()
                .ExportField("IntValue", "V1");

            var catalog =
               new ConventionCatalog(registry);

            var container =
                new CompositionContainer(catalog);

            var exportedValue = container.GetExportedValue<int>("V1");
            Assert.That(exportedValue, Is.EqualTo(1234));
        }

        [Test]
        public void ConventionCatalog_should_support_property_imports()
        {
            var registry = new PartRegistry();
            registry.TypeScanner = new AssemblyTypeScanner(Assembly.GetExecutingAssembly());

            registry
                .Part()
                .ForType<SampleExport>()
                .ExportProperty("TextValue", "V1");

            registry
                .Part()
                .ForType<SampleImport>()
                .Export()
                .ImportProperty("TextValue", "V1");

            var catalog =
               new ConventionCatalog(registry);

            var container =
                new CompositionContainer(catalog);

            var exportedValue = container.GetExportedValue<SampleImport>();
            exportedValue.ShouldNotBeNull();
            exportedValue.TextValue.ShouldEqual("this is some text");
        }

        [Test]
        public void ConventionCatalog_should_support_field_imports()
        {
            var registry = new PartRegistry();
            registry.TypeScanner = new AssemblyTypeScanner(Assembly.GetExecutingAssembly());

            registry
                .Part()
                .ForType<SampleExport>()
                .ExportField("IntValue", "V1");

            registry
                .Part()
                .ForType<SampleImport>()
                .Export()
                .ImportField("IntValue", "V1");

            var catalog =
               new ConventionCatalog(registry);

            var container =
                new CompositionContainer(catalog);

            var exportedValue = container.GetExportedValue<SampleImport>();
            exportedValue.ShouldNotBeNull();
            exportedValue.IntValue.ShouldEqual(1234);
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

    public class CtorRegistry : PartRegistry
    {
        public CtorRegistry()
        {
            Scan(x => {
                x.Assembly(Assembly.GetExecutingAssembly());
            });

            Part()
                .ForType<InjectedHost>()
                .Export()
                .ImportConstructor()
                .MakeShared();

            Part()
                .ForTypesAssignableFrom<IWidget>()
                .ExportAs<IWidget>()
                .MakeShared();
        }
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
