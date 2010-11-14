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
                .ForTypesAssignableFrom<InjectedHost>()
                .ExportTypeAs<InjectedHost>()
                .ImportConstructor()
                .MakeShared();

            Part()
                .ForTypesAssignableFrom<IWidget>()
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
