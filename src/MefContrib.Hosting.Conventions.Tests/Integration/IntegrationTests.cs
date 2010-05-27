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
        public void ConventionCatalog_should_support_constructor_injection()
        {
            var loader =
                new TypeLoader();
            loader.AddTypes(() => Assembly.GetExecutingAssembly().GetExportedTypes());

            var catalog =
                new ConventionCatalog(new[] { new CtorRegistry() }, loader);

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
            Part()
                .ForTypesMatching(x => x.Equals(typeof(InjectedHost)))
                .ExportTypeAs<InjectedHost>()
                .ImportConstructor()
                .MakeShared();

            Part()
                .ForTypesMatching(x => x.GetInterfaces().Contains(typeof(IWidget)))
                .ExportTypeAs<IWidget>()
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
