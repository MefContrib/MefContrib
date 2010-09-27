using MefContrib.Tests;

namespace MefContrib.Hosting.Conventions.Tests
{
    using System.ComponentModel.Composition;
    using System.Linq;
    using NUnit.Framework;

    [TestFixture]
    public class MefIntegrationTests
    {
        [Test]
        public void Registry_should_work()
        {
            var registry =
                new FakeConventionRegistry();

            var conventions =
                registry.GetConventions();

            conventions.Count().ShouldEqual(1);
        }

        //[Fact(Skip = "Integration test")]
        //public void ConventionsCatalog_should_export_conventionpart()
        //{
        //    // Setup conventions using the semantic model
        //    // This is NOT the API that the user will be exposed to,
        //    // there will be a DSL at the front

        //    var exportConvention =
        //        new ExportConvention
        //        {
        //            Members = t => new[] { t },
        //            ContractType = x => typeof(IConventionPart),
        //        };

        //    var importConvention =
        //        new ImportConvention
        //        {
        //            Members = t => new[] { ReflectionServices.GetProperty<IConventionPart>(p => p.Logger) },
        //            ContractType = x => typeof(ILogger)
        //        };

        //    var convention =
        //        new PartConvention();

        //    convention.Imports.Add(importConvention);
        //    convention.Exports.Add(exportConvention);
        //    convention.Condition = t => t.GetInterfaces().Contains(typeof(IConventionPart));

        //    var exportConvention2 =
        //        new ExportConvention
        //        {
        //            Members = t => new[] { typeof(NullLogger) },
        //            ContractType = x => typeof(ILogger),
        //        };

        //    var convention2 =
        //        new PartConvention();
        //    convention2.Exports.Add(exportConvention2);
        //    convention2.Condition = t => t.GetInterfaces().Contains(typeof(ILogger));

        //    var model =
        //        new PartConventionsContainer();

        //    model.Conventions.Add(convention);
        //    model.Conventions.Add(convention2);

        //    // Setup container

        //    //var conventionCatalog =
        //    //    new ConventionsCatalog(model);

        //    ConventionsCatalog conventionCatalog = null;

        //    var typeCatalog =
        //        new TypeCatalog(typeof(AttributedPart));

        //    var aggregated =
        //        new AggregateCatalog(typeCatalog, conventionCatalog);

        //    var container =
        //        new CompositionContainer(aggregated);

        //    var part = new AttributedPart();

        //    var batch =
        //        new CompositionBatch();
        //    batch.AddPart(part);

        //    container.Compose(batch);

        //    // Assert

        //    part.Part.Count().ShouldEqual(2);
        //}
    }

    public class AttributedPart
    {
        [ImportMany(typeof(IConventionPart))]
        public IConventionPart[] Part { get; set; }
    }

    public interface IConventionPart
    {
        string Name { get; }

        ILogger Logger { get; set; }
    }

    public class ConventionPart : IConventionPart
    {
        public string Name
        {
            get { return "ConventionPart"; }
        }

        public ILogger Logger { get; set; }
    }

    public class OtherConventionPart : IConventionPart
    {
        public string Name
        {
            get { return "OtherConventionPart"; }
        }

        public ILogger Logger { get; set; }
    }

    public interface ILogger
    {
        void Add();
    }

    public class NullLogger : ILogger
    {
        public void Add()
        {
        }
    }
}